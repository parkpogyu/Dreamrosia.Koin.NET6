using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class DelistingSymbolJob : IJob
    {
        private readonly IDelistingSymbolService _delistingSymbolService;
        private readonly ILogger<DelistingSymbolJob> _logger;

        public DelistingSymbolJob(IDelistingSymbolService delistingSymbolService,
                                  ILogger<DelistingSymbolJob> logger)
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
