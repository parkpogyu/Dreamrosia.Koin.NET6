using Dreamrosia.Koin.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Schedules
{
    [DisallowConcurrentExecution]
    public class SynchronizeJob : IJob
    {

        private readonly ISynchronizeService _synchronizeService;
        private readonly ILogger<SynchronizeJob> _logger;

        public SynchronizeJob(ISynchronizeService synchronizeService,
                              ILogger<SynchronizeJob> logger)
        {
            _synchronizeService = synchronizeService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _synchronizeService.GetTradingTermsAsync();

                List<Task> tasks = new List<Task>();

                tasks.Add(_synchronizeService.SavePositionsAsync());
                tasks.Add(_synchronizeService.SaveOrdersAsync());
                tasks.Add(_synchronizeService.SaveTransfersAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
