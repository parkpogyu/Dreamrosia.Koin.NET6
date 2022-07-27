using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class MarketJob : IJob
    {
        private readonly IUPbitSymbolService _upbitSymbolService;
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly IUPbitCandleService _upbitCandleService;
        private readonly IUPbitCrixService _upbitCrixService;
        private readonly IUPbitMarketIndexService _upbitMarketIndexService;
        private readonly ICandleService _candleService;
        private readonly ISeasonSignalService _seasonSignalService;
        private readonly IDelistingSymbolService _delistingSymbolService;
        private readonly IUnlistedSymbolService _unlistedSymbolService;
        private readonly ILogger<MarketJob> _logger;

        public MarketJob(IUPbitSymbolService upbitSymbolService,
                         IUPbitTickerService upbitTickerService,
                         IUPbitCandleService upbitCandleService,
                         IUPbitCrixService upbitCrixService,
                         IUPbitMarketIndexService upbitMarketIndexService,
                         ICandleService candleService,
                         ISeasonSignalService seasonSignalService,
                         IDelistingSymbolService delistingSymbolService,
                         IUnlistedSymbolService unlistedSymbolService,
                         ILogger<MarketJob> logger)
        {
            _upbitSymbolService = upbitSymbolService;
            _upbitTickerService = upbitTickerService;
            _upbitCandleService = upbitCandleService;
            _upbitCrixService = upbitCrixService;
            _upbitMarketIndexService = upbitMarketIndexService;
            _candleService = candleService;
            _seasonSignalService = seasonSignalService;
            _delistingSymbolService = delistingSymbolService;
            _unlistedSymbolService = unlistedSymbolService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                tasks.Add(TickerToCandleJobAsync());
                tasks.Add(SeasonSignalJobAsync());
                tasks.Add(SymbolJobAsync());
                tasks.Add(CandleJobAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task TickerToCandleJobAsync()
        {
            try
            {
                DateTime utc = DateTime.UtcNow;

                // 00:00 ~ 00:05 2초간격, 이외는 정분에 작용
                // 00:[00-05]:ss/2,  hh:mm:00
                if (utc.Hour == 0)
                {
                    if (utc.Minute > 5 && utc.Second != 0) { return; }
                }
                else
                {
                    if (utc.Second != 0) { return; }
                }

                var response = await _upbitTickerService.GetCandlesAsync();

                if (!response.Succeeded) { return; }

                var candles = response.Data
                                      .Where(f => f.candle_date_time_utc == utc.Date)
                                      .ToArray();

                if (!candles.Any()) { return; }

                await _candleService.SaveCandlesAsync(candles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task SeasonSignalJobAsync()
        {
            try
            {
                DateTime utc = DateTime.UtcNow;

                // 00:00 ~ 00:05 2초간격, 이외는 정분에 작용
                // 00:[00-05]:ss/2,  hh:mm:00
                if (utc.Hour == 0)
                {
                    if (utc.Minute > 5 && utc.Second != 0) { return; }
                }
                else
                {
                    if (utc.Second != 0) { return; }
                }

                await _seasonSignalService.UpdateSeasonSignalsAsync(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task SymbolJobAsync()
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                // 정분 장업
                // hh:mm:02
                if (now.Second != 2) { return; }

                List<Task> tasks = new List<Task>();

                tasks.Add(_upbitSymbolService.GetSymbolsAsync());
                tasks.Add(_upbitCrixService.GetCrixesAsync());
                tasks.Add(_upbitMarketIndexService.GetMarketIndicesAsync());
                tasks.Add(_delistingSymbolService.CollectDelistingSymbolsAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task CandleJobAsync()
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                // 23:50 ~ 24:00 사이 2분 간격 수집
                // hh:mm:04
                if (now.Hour != 23 || now.Minute < 50 || now.Minute % 2 == 0 || now.Second != 4) { return; }

                await _upbitCandleService.GetCandlesAsync(TimeFrames.Week);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task UnlistedSymbolJobAsync()
        {
            await _unlistedSymbolService.FilterUnlistedSymbolsAsync();
            await _unlistedSymbolService.GetSymbolsIdAsync();
            await _unlistedSymbolService.GetPriceAsync();
        }
    }
}
