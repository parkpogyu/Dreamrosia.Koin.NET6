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
    public class MarketIndexService : IMarketIndexService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MarketIndexService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public MarketIndexService(IUnitOfWork<int> unitOfWork,
                                  BlazorHeroContext context,
                                  IMapper mapper,
                                  ILogger<MarketIndexService> logger,
                                  IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<MarketIndexDto>>> GetMarketIndicesAsync(DateTime head, DateTime rear)
        {
            try
            {
                var items = await _context.MarketIndices
                                          .AsNoTracking()
                                          .Where(f => f.candleDateTimeUtc >= head.Date &&
                                                      f.candleDateTimeUtc <= rear.Date)
                                          .OrderByDescending(f => f.candleDateTimeUtc)
                                          .ToArrayAsync();

                return await Result<IEnumerable<MarketIndexDto>>.SuccessAsync(_mapper.Map<IEnumerable<MarketIndexDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<MarketIndexDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SaveMarketIndicesAsync(IEnumerable<MarketIndex> models)
        {
            try
            {
                var entities = _unitOfWork.Repository<MarketIndex>().Entities;

                var items = (from lt in models
                             from rt in entities.Where(f => f.code.Equals(lt.code) &&
                                                            f.candleDateTimeUtc == lt.candleDateTimeUtc).DefaultIfEmpty()
                             orderby lt.candleDateTimeUtc, lt.code
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<MarketIndex>().AddAsync(_mapper.Map<MarketIndex>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<MarketIndex>().UpdateAsync(old);
                    }
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result<int>.SuccessAsync(models.Count(), string.Format(_localizer["{0} Updated"], _localizer["MarketIndices"]));

            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result<int>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}