using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class UnlistedSymbolJob : IJob
    {
        private readonly IUnlistedSymbolService _unlistedSymbolService;
        private readonly ILogger<UnlistedSymbolJob> _logger;

        public UnlistedSymbolJob(IUnlistedSymbolService unlistedSymbolService,
                                 ILogger<UnlistedSymbolJob> logger)
        {
            _unlistedSymbolService = unlistedSymbolService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _unlistedSymbolService.FilterUnlistedSymbolsAsync();
                await _unlistedSymbolService.GetSymbolsIdAsync();
                await _unlistedSymbolService.GetPriceAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
