using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class MappingMiningBotJob : IJob
    {
        private readonly IMiningBotService _miningBotService;
        private readonly ILogger<DetermineSeasonSignalJob> _logger;

        public MappingMiningBotJob(IMiningBotService miningBotService,
                                   ILogger<DetermineSeasonSignalJob> logger)
        {
            _miningBotService = miningBotService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _miningBotService.MappingMiningBotAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
