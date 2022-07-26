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
        private readonly IUPbitMarketIndexService _upbitMarketIndexService;
        private readonly IDelistingSymbolService _delistingSymbolService;
        private readonly IUnlistedSymbolService _unlistedSymbolService;
        private readonly ISeasonSignalService _seasonSignalService;
        private readonly IUPbitCrixService _upbitCrixService;
        private readonly IUPbitSymbolService _upbitSymbolService;
        private readonly IUPbitCandleService _upbitCandleService;
        private readonly ILogger<MarketJob> _logger;

        public MarketJob(IUPbitMarketIndexService upbitMarketIndexService,
                         IDelistingSymbolService delistingSymbolService,
                         IUnlistedSymbolService unlistedSymbolService,
                         ISeasonSignalService seasonSignalService,
                         IUPbitCrixService upbitCrixService,
                         IUPbitSymbolService upbitSymbolService,
                         IUPbitCandleService upbitCandleService,
                         ILogger<MarketJob> logger)
        {
            _delistingSymbolService = delistingSymbolService;
            _upbitMarketIndexService = upbitMarketIndexService;
            _unlistedSymbolService = unlistedSymbolService;
            _seasonSignalService = seasonSignalService;
            _upbitCrixService = upbitCrixService;
            _upbitSymbolService = upbitSymbolService;
            _upbitCandleService = upbitCandleService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                tasks.Add(_upbitCrixService.GetCrixesAsync());
                tasks.Add(_upbitSymbolService.GetSymbolsAsync());
                tasks.Add(_upbitMarketIndexService.GetMarketIndicesAsync());

                DateTime now = DateTime.Now;
                //if (now.Hour == 8 && 50 <= now.Minute && now.Minute % 2 == 0)
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
