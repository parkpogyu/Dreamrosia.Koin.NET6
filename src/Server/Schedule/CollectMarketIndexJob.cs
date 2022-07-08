using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class CollectMarketIndexJob : IJob
    {
        private readonly IUPbitMarketIndexService _upbitMarketIndexService;
        private readonly ILogger<CollectMarketIndexJob> _logger;

        public CollectMarketIndexJob(IUPbitMarketIndexService upbitMarketIndexService,
                                     ILogger<CollectMarketIndexJob> logger)
        {
            _upbitMarketIndexService = upbitMarketIndexService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _upbitMarketIndexService.GetMarketIndicesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
