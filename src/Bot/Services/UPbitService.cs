using AutoMapper;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Extentions;
using Dreamrosia.Koin.Bot.Interfaces;
using Dreamrosia.Koin.Bot.Models;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Services
{
    public class UPbitService : IUPbitService
    {
        private readonly ISynchronizeService _synchronizeService;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitService> _logger;

        public UPbitService(ISynchronizeService synchronizeService,
                            IMapper mapper,
                            ILogger<UPbitService> logger)
        {
            _synchronizeService = synchronizeService;
            _mapper = mapper;
            _logger = logger;
        }

        private bool IsCanProcess()
        {
            if (Depot.HasFatalError) { return false; }
            if (Depot.TradingTerms is null) { return false; }
            if (Depot.TradingTerms.UPbitKey is null) { return false; }

            ExchangeClientKeys.SetAuthenticationKey(_mapper.Map<UPbitKey>(Depot.TradingTerms.UPbitKey));

            return true;
        }

        public async Task GetPositionsAsync()
        {
            if (!IsCanProcess()) { return; }

            ExPositions exPositions = new ExPositions();

            var result = await exPositions.GetPositionsAsync();

            if (result.Succeeded)
            {
                Depot.SetPostions(_mapper.Map<IEnumerable<PositionDto>>(result.Data));
            }
            else
            {
                _logger.LogWarning($"GetPositionsAsync: {result.FullMessage}");

                await CheckFatalError(result);
            }
        }

        public async Task GetCompletedOrdersAsync()
        {
            if (!IsCanProcess()) { return; }

            ExOrders exOrders = new ExOrders();

            var to1 = DateTime.Now.FirstDayOfWeek(DayOfWeek.Monday);
            var to2 = Convert.ToDateTime(Depot.TradingTerms.LastOrder?.created_at);

            var to = (to1 < to2 ? to1 : to2).Date;

            var result = await exOrders.GetCompletedOrdersAsync(to);

            if (result.Succeeded)
            {
                await GetTradesAsync(result.Data);

                Depot.SetThisWeekOrders(_mapper.Map<IEnumerable<OrderDto>>(result.Data));
                Depot.SetCompletedOrders(_mapper.Map<IEnumerable<OrderDto>>(result.Data));
            }
            else
            {
                _logger.LogWarning($"GetCompletedOrdersAsync: {result.FullMessage}");

                await CheckFatalError(result);
            }
        }

        private async Task GetTradesAsync(IEnumerable<Order> orders)
        {
            var items = (from lt in orders
                         from rt in Depot.StoredOrders.Where(f => f.uuid.Equals(lt.uuid)).DefaultIfEmpty()
                         select ((Func<Order>)(() =>
                         {
                             if (rt is not null)
                             {
                                 if (rt.trades_count == lt.trades_count)
                                 {
                                     lt.exec_amount = rt.exec_amount;
                                     lt.avg_price = rt.avg_price;
                                 }
                             }

                             return lt;

                         }))()).Where(f => f.trades_count != 0 && f.avg_price == 0).ToArray();

            if (items.Any())
            {
                ExOrders exOrders = new ExOrders();

                await exOrders.GetTradesAsync(items);
            }
        }

        public async Task GetWaitingOrdersAsync()
        {
            if (!IsCanProcess()) { return; }

            ExOrders exOrders = new ExOrders();

            var result = await exOrders.GetWaitingOrdersAsync();

            if (result.Succeeded)
            {
                await GetTradesAsync(result.Data);

                Depot.SetWaitingOrders(_mapper.Map<IEnumerable<OrderDto>>(result.Data));
            }
            else
            {
                _logger.LogWarning($"GetWaitingOrdersAsync: {result.FullMessage}");

                await CheckFatalError(result);
            }
        }

        public async Task GetDepositsAsync()
        {
            if (!IsCanProcess()) { return; }

            ExTransfers exTransfers = new ExTransfers();

            var result = await exTransfers.GetDepositTransfersAsync(Depot.TradingTerms.LastDeposit?.created_at);

            if (result.Succeeded)
            {

                Depot.SetDeposits(_mapper.Map<IEnumerable<TransferDto>>(result.Data));
            }
            else
            {
                _logger.LogWarning($"GetDepositsAsync: {result.FullMessage}");

                await CheckFatalError(result);
            }
        }

        public async Task GetWithdrawsAsync()
        {
            if (!IsCanProcess()) { return; }

            ExTransfers exTransfers = new ExTransfers();

            var result = await exTransfers.GetWithdrawTransfersAsync(Depot.TradingTerms.LastWithdraw?.created_at);

            if (result.Succeeded)
            {
                Depot.SetWithdraws(_mapper.Map<IEnumerable<TransferDto>>(result.Data));
            }
            else
            {
                _logger.LogWarning($"GetWithdrawsAsync: {result.FullMessage}");

                await CheckFatalError(result);
            }
        }

        private async Task CheckFatalError(IResult result)
        {
            if (!ClientConstants.FatalErrors.ContainsKey(result.Code ?? string.Empty)) { return; }

            Depot.OccurredFatalError();

            await _synchronizeService.OccurredFatalErrorAsync(result.Code, result.FullMessage);
        }
    }
}