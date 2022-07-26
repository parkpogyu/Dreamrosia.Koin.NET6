using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class MarketJob : IJob
    {
        private readonly IUPbitSymbolService _upbitSymbolService;
        private readonly IUPbitCandleService _upbitCandleService;
        private readonly IUPbitCrixService _upbitCrixService;
        private readonly IUPbitMarketIndexService _upbitMarketIndexService;
        private readonly ISeasonSignalService _seasonSignalService;
        private readonly IDelistingSymbolService _delistingSymbolService;
        private readonly IUnlistedSymbolService _unlistedSymbolService;
        private readonly ILogger<MarketJob> _logger;

        public MarketJob(IUPbitSymbolService upbitSymbolService,
                         IUPbitCandleService upbitCandleService,
                         IUPbitCrixService upbitCrixService,
                         IUPbitMarketIndexService upbitMarketIndexService,
                         ISeasonSignalService seasonSignalService,
                         IDelistingSymbolService delistingSymbolService,
                         IUnlistedSymbolService unlistedSymbolService,
                         ILogger<MarketJob> logger)
        {
            _upbitSymbolService = upbitSymbolService;
            _upbitCandleService = upbitCandleService;
            _upbitCrixService = upbitCrixService;
            _upbitMarketIndexService = upbitMarketIndexService;
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

                tasks.Add(_upbitSymbolService.GetSymbolsAsync());
                tasks.Add(_upbitCrixService.GetCrixesAsync());
                tasks.Add(_upbitMarketIndexService.GetMarketIndicesAsync());

                DateTime now = DateTime.UtcNow;
                // 23:50 ~ 24:00 사이 2분 간격 수집
                if (now.Hour == 23 && (50 < now.Minute && now.Minute % 2 == 0))
                {
                    await _upbitCandleService.GetCandlesAsync(TimeFrames.Week);
                }

                tasks.Add(_delistingSymbolService.CollectDelistingSymbolsAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);

                await _seasonSignalService.UpdateSeasonSignalsAsync(null);
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
