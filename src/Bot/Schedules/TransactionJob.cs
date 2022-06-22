using Dreamrosia.Koin.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Schedules
{
    [DisallowConcurrentExecution]
    public class TransactionJob : IJob
    {
        private readonly IUPbitService _upbitService;
        private readonly ITradeOrderService _tradeOrderService;
        private readonly ISynchronizeService _synchronizeService;

        private readonly ILogger<TransactionJob> _logger;

        public TransactionJob(IUPbitService upbitService,
                              ITradeOrderService tradeOrderService,
                              ISynchronizeService synchronizeService,
                              ILogger<TransactionJob> logger)
        {
            _upbitService = upbitService;
            _tradeOrderService = tradeOrderService;
            _synchronizeService = synchronizeService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                List<Task> tasks = new List<Task>();

                tasks.Add(_synchronizeService.ConnectHubAsync());

                tasks.Add(_upbitService.GetPositionsAsync());
                tasks.Add(_upbitService.GetCompletedOrdersAsync());
                tasks.Add(_upbitService.GetWaitingOrdersAsync());
                tasks.Add(_upbitService.GetDepositsAsync());
                tasks.Add(_upbitService.GetWithdrawsAsync());

                await Task.WhenAll(tasks).ConfigureAwait(false);

                await _tradeOrderService.DoTradeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}