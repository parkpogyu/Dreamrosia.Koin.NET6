using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Extensions;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class SeasonSignalService : ISeasonSignalService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SeasonSignalService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public SeasonSignalService(IServiceProvider serviceProvider,
                                   IUnitOfWork<int> unitOfWork,
                                   BlazorHeroContext context,
                                   IMapper mapper,
                                   ILogger<SeasonSignalService> logger,
                                   IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<SeasonSignalDto>>> GetSeasonSignalsAsync(string userId)
        {
            try
            {
                var items = await _context.SeasonSignals
                                          .AsNoTracking()
                                          .Where(f => f.UserId.Equals(userId))
                                          .ToArrayAsync();

                return await Result<IEnumerable<SeasonSignalDto>>.SuccessAsync(_mapper.Map<IEnumerable<SeasonSignalDto>>(items));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<SeasonSignalDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> UpdateSeasonSignalsAsync(string userId)
        {
            try
            {
                var symbols = _mapper.Map<IEnumerable<SymbolDto>>(await _context.Symbols
                                                                                .AsNoTracking()
                                                                                .ToArrayAsync());

                var signals = _unitOfWork.Repository<SeasonSignal>().Entities;

                var items = (from lt in symbols
                             from rt in signals.AsNoTracking()
                                               .Where(f => f.UserId.Equals(userId) &&
                                                           f.market.Equals(lt.market)).DefaultIfEmpty()
                             select ((Func<SeasonSignalDto>)(() =>
                             {
                                 var item = rt is null ?
                                            new SeasonSignalDto() { market = lt.market } :
                                            _mapper.Map<SeasonSignalDto>(rt);

                                 return item;

                             }))()).ToArray();

                Parallel.ForEach(items, async item =>
                {
                    await GenerateSeasonSignals(item);
                });

                var saved = await SaveSeasonSignalsAsync(items);

                if (saved.Succeeded)
                {
                    return await Result.SuccessAsync();
                }
                else
                {
                    _logger.LogWarning($"SaveSeasonSignalsAsync: {saved.FullMessage}");

                    return await Result.FailAsync(saved.Messages);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        private async Task GenerateSeasonSignals(SeasonSignalDto signal)
        {
            var today = DateTime.UtcNow.Date;

            if (today < signal.UpdatedAt.ToUniversalTime()) { return; }

            using var scope = _serviceProvider.CreateScope();

            var _candleService = scope.ServiceProvider.GetRequiredService<ICandleService>();
            var _macdService = scope.ServiceProvider.GetRequiredService<IMACDService>();

            var result = await _candleService.GetCandlesAsync(signal.market,
                                                              new DateTime().ToUniversalDate(),
                                                              today);

            if (!result.Succeeded || !result.Data.Any()) { return; }

            var candles = result.Data.OrderBy(f => f.candle_date_time_utc);

            var last = candles.Last();

            if (last.candle_date_time_utc != today) { return; }

            if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
            {
                _macdService.Generate(candles.GetTimeFrameCandles(TimeFrames.Week));

                signal.WeeklySignal = _macdService.HistogramState(1);
            }

            _macdService.Generate(candles);

            signal.DailySignal = _macdService.HistogramState(1);
            signal.UpdatedAt = DateTime.Now;

            _logger.LogInformation($"{signal.market}, W:{signal.WeeklySignal.ToDescriptionString()}, D:{ signal.DailySignal.ToDescriptionString()}");
        }

        private async Task<IResult> SaveSeasonSignalsAsync(IEnumerable<SeasonSignalDto> models)
        {
            try
            {
                var entities = _unitOfWork.Repository<SeasonSignal>().Entities;

                var items = (from lt in models
                             from rt in entities.Where(f => f.UserId.Equals(lt.UserId) &&
                                                            f.market.Equals(lt.market)).DefaultIfEmpty()
                             orderby lt.market
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<SeasonSignal>().AddAsync(_mapper.Map<SeasonSignal>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<SeasonSignal>().UpdateAsync(old);
                    }
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["SeasonSignals"]));

            }
            catch (Exception)
            {
                await _unitOfWork.Rollback();

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}