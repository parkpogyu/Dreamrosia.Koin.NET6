using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class CandleService : ICandleService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CandleService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public CandleService(IUnitOfWork<int> unitOfWork,
                             BlazorHeroContext context,
                             IMapper mapper,
                             ILogger<CandleService> logger,
                             IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }


        public async Task<IResult<IEnumerable<CandleDto>>> GetCandlesAsync(string market, DateTime head, DateTime rear)
        {
            try
            {
                var items = await _context.Candles
                                          .AsNoTracking()
                                          .Where(f => f.market.Equals(market) &&
                                                      f.candle_date_time_utc >= head.Date &&
                                                      f.candle_date_time_utc <= rear.Date)
                                          .OrderByDescending(f => f.candle_date_time_utc)
                                          .ToArrayAsync();

                return await Result<IEnumerable<CandleDto>>.SuccessAsync(_mapper.Map<IEnumerable<CandleDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<CandleDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<LastCandleDto>>> GetLastCandlesAsync(IEnumerable<string> markets)
        {
            try
            {
                List<LastCandleDto> responses = new List<LastCandleDto>();

                foreach (var market in markets)
                {
                    var candle = await _context.Candles
                                               .AsNoTracking()
                                               .Where(f => f.market.Equals(market))
                                               .OrderByDescending(f => f.candle_date_time_utc)
                                               .FirstOrDefaultAsync();

                    responses.Add(new LastCandleDto()
                    {
                        market = market,
                        Candle = _mapper.Map<CandleDto>(candle),
                    });
                }

                return await Result<IEnumerable<LastCandleDto>>.SuccessAsync(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<LastCandleDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SaveCandlesAsync(IEnumerable<CandleDto> models)
        {
            try
            {
                var entities = _unitOfWork.Repository<Candle>().Entities;

                var items = (from lt in models
                             from rt in entities.Where(f => f.market.Equals(lt.market) &&
                                                            f.candle_date_time_utc == lt.candle_date_time_utc).DefaultIfEmpty()
                             orderby lt.candle_date_time_utc, lt.market
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Candle>().AddAsync(_mapper.Map<Candle>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Candle>().UpdateAsync(old);
                    }
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Candles"]));

            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}