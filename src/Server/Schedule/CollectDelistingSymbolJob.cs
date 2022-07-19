using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class CollectDelistingSymbolJob : IJob
    {
        private readonly IDelistingSymbolService _delistingSymbolService;
        private readonly ILogger<CollectDelistingSymbolJob> _logger;

        public CollectDelistingSymbolJob(IDelistingSymbolService delistingSymbolService,
                                         ILogger<CollectDelistingSymbolJob> logger)
        {
            _delistingSymbolService = delistingSymbolService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _delistingSymbolService.CollectDelistingSymbolsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
