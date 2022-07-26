using Dreamrosia.Koin.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Schedules
{
    [DisallowConcurrentExecution]
    public class TickerJob : IJob
    {
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly ICandleService _candleService;
        private readonly ILogger<TickerJob> _logger;

        public TickerJob(IUPbitTickerService upbitTickerService,
                         ICandleService candleService,
                         ILogger<TickerJob> logger)
        {
            _upbitTickerService = upbitTickerService;
            _candleService = candleService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime utc = DateTime.UtcNow;

                // 00:00 ~ 00:05 2초간격, 이외는 정분에 작용
                if (!(utc.Hour == 0 && utc.Minute < 5) && utc.Second != 0) { return; }

                var response = await _upbitTickerService.GetCandlesAsync();

                if (!response.Succeeded) { return; }

                var candles = response.Data
                                      .Where(f => f.candle_date_time_utc == utc.Date)
                                      .ToArray();

                if (!candles.Any()) { return; }

                await _candleService.SaveCandlesAsync(candles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
