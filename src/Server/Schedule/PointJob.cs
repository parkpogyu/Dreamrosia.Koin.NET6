using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class PointJob : IJob
    {
        private readonly IPointService _pointService;
        private readonly ILogger<PointJob> _logger;

        public PointJob(IPointService pointService,
                        ILogger<PointJob> logger)
        {
            _pointService = pointService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _pointService.DailyDeductPoint();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
