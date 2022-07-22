using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class SeasonSignalJob : IJob
    {
        private readonly IUPbitCrixService _upbitCrixService;
        private readonly IUPbitSymbolService _upbitSymbolService;
        private readonly IUPbitCandleService _upbitCandleService;
        private readonly ISeasonSignalService _seasonSignalService;
        private readonly ILogger<SeasonSignalJob> _logger;

        public SeasonSignalJob(IUPbitCrixService upbitCrixService,
                               IUPbitSymbolService upbitSymbolService,
                               IUPbitCandleService upbitCandleService,
                               ISeasonSignalService seasonSignalService,
                               ILogger<SeasonSignalJob> logger)
        {
            _upbitCrixService = upbitCrixService;
            _upbitSymbolService = upbitSymbolService;
            _upbitCandleService = upbitCandleService;
            _seasonSignalService = seasonSignalService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                tasks.Add(_upbitCrixService.GetCrixesAsync());
                tasks.Add(_upbitSymbolService.GetSymbolsAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);

                await _upbitCandleService.GetCandlesAsync();
                await _seasonSignalService.UpdateSeasonSignalsAsync(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
