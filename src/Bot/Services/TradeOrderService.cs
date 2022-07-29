using AutoMapper;
using Dreamrosia.Koin.Bot.Constants;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Extensions;
using Dreamrosia.Koin.Bot.Interfaces;
using Dreamrosia.Koin.Bot.Models;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Services
{
    public class TradeOrderService : ITradeOrderService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TradeOrderService> _logger;

        private TradingTermsDto TradingTerms => Depot.TradingTerms;
        private UPbitKeyDto UPbitKey => Depot.TradingTerms?.UPbitKey;
        private IEnumerable<OrderDto> ThisWeekOrders => Depot.ThisWeekOrders;
        private IEnumerable<OrderDto> WaitingOrders => Depot.WaitingOrders;
        private IEnumerable<PositionDto> Coins => Depot.Positions?.Where(f => f.avg_buy_price > 0).ToArray();
        private PositionDto Cash => Depot.Positions?.Where(f => f.code.Equals("KRW")).SingleOrDefault();
        private IEnumerable<SeasonSignalDto> Signals => Depot.TradingTerms?.Signals;

        private readonly List<Order> OriginalBidOrders = new List<Order>();

        /// <summary>
        /// 포지션이 정상 적용 시 까지 대기
        /// </summary>
        private readonly int DelayPositionCheckTime = 3;

        private int BidAmount { get; set; }

        public TradeOrderService(IMapper mapper,
                                 ILogger<TradeOrderService> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        private IEnumerable<string> GetBidSideSymbols()
        {
            IEnumerable<SeasonSignalDto> items = new List<SeasonSignalDto>();

            if (TradingTerms is not null)
            {
                if (TradingTerms.TimeFrame == TimeFrames.Day)
                {
                    items = Signals?.Where(f => f.DailySignal == SeasonSignals.GoldenCross);
                }
                else if (TradingTerms.TimeFrame == TimeFrames.Week)
                {
                    items = Signals?.Where(f => (f.WeeklySignal == SeasonSignals.GoldenCross ||
                                                (f.WeeklySignal == SeasonSignals.Above &&
                                                 f.DailySignal == SeasonSignals.GoldenCross)));
                }
            }

            return items.Select(f => f.market).ToArray();
        }

        private IEnumerable<string> GetAskSideSymbols()
        {
            IEnumerable<SeasonSignalDto> items = new List<SeasonSignalDto>();

            if (TradingTerms is not null)
            {
                if (TradingTerms.TimeFrame == TimeFrames.Day)
                {
                    items = Signals?.Where(f => f.DailySignal == SeasonSignals.Below ||
                                                f.DailySignal == SeasonSignals.DeadCross);
                }
                else if (TradingTerms.TimeFrame == TimeFrames.Week)
                {
                    items = Signals?.Where(f => f.WeeklySignal == SeasonSignals.Below ||
                                                f.WeeklySignal == SeasonSignals.DeadCross);
                }
            }

            return items.Select(f => f.market).ToArray();
        }

        private IEnumerable<OrderPostParameterDto> GetAskOrders()
        {
            var sellables = (from coin in Coins.Where(f => TradingConstants.MinOrderableAmount < f.BalEvalAmt && 0 < f.balance)
                             from order in OriginalBidOrders.Where(f => f.market.Equals(coin.market)).DefaultIfEmpty()
                             where order is null
                             select coin).ToArray();

            List<OrderPostParameterDto> orders = new List<OrderPostParameterDto>();

            if (TradingTerms.LiquidatePositions)
            {
                foreach (var item in sellables)
                {
                    orders.Add(new OrderPostParameterDto()
                    {
                        side = OrderSide.ask,
                        market = item.market,
                        ord_type = OrderType.market,
                        volume = item.balance,
                        BalEvalAmt = item.BalEvalAmt,
                        Remark = $"{OrderReason.Liquidate.ToDescriptionString()}, {item.PnLRat:N2}%",
                    });
                }

                return orders;
            }

            #region 신호 매도
            var items = (from coin in sellables
                         from ask in GetAskSideSymbols().Where(f => f.Equals(coin.market)).DefaultIfEmpty()
                         where ask is not null
                         select coin).ToArray();

            foreach (var item in items)
            {
                orders.Add(new OrderPostParameterDto()
                {
                    side = OrderSide.ask,
                    market = item.market,
                    ord_type = OrderType.market,
                    volume = item.balance,
                    BalEvalAmt = item.BalEvalAmt,
                    Remark = $"{OrderReason.Signal.ToDescriptionString()}, {item.PnLRat:N2}%",
                });
            }
            #endregion

            #region 매도 조건
            foreach (var item in sellables)
            {
                var reason = GetCutOffReason(item);

                if (reason != OrderReason.None)
                {
                    orders.Add(new OrderPostParameterDto()
                    {
                        side = OrderSide.ask,
                        market = item.market,
                        ord_type = OrderType.market,
                        volume = item.balance,
                        BalEvalAmt = item.BalEvalAmt,
                        Remark = $"{ reason.ToDescriptionString()}, {GetPriceText(item.trade_price)} / {GetPriceText(item.avg_buy_price)}, {item.PnLRat:N2}%",
                    });
                }
            }

            OrderReason GetCutOffReason(PositionDto item)
            {
                OrderReason reason = OrderReason.None;

                if (TradingTerms.UseStopLoss && -100F < item.PnLRat && item.PnLRat < TradingTerms.StopLoss)
                {
                    reason = OrderReason.StopLoss;
                }
                else if (TradingTerms.UseTakeProfit && TradingTerms.TakeProfit < item.PnLRat && float.IsFinite(item.PnLRat))
                {
                    reason = OrderReason.TakeProfit;
                }

                return reason;
            }
            #endregion

            return orders.GroupBy(g => g.market).Select(f => f.First());
        }

        private IEnumerable<OrderPostParameterDto> GetBidOrders(IEnumerable<OrderPostParameterDto> asks)
        {
            if (TradingTerms.LiquidatePositions) { return new List<OrderPostParameterDto>(); }

            IEnumerable<string> bids = GetBidSideSymbols().Except(asks.Select(f => f.market))
                                                          .Except(OriginalBidOrders.Select(f => f.market))
                                                          .Except(WaitingOrders.Select(f => f.market)).ToArray();

            if (TradingTerms.TimeFrame == TimeFrames.Day)
            {
                DateTime utc = DateTime.UtcNow.Date;

                bids = bids.Except(ThisWeekOrders.Where(f => f.created_at.ToUniversalTime().Date == utc.Date)
                                                 .Select(f => f.market)).ToArray();
            }
            else if (TradingTerms.TimeFrame == TimeFrames.Week)
            {
                bids = bids.Except(ThisWeekOrders.Select(f => f.market)).ToArray();
            }
            else
            {
                return new List<OrderPostParameterDto>();
            }

            var items = (from bid in bids
                         from coin in Coins.Where(f => f.market.Equals(bid)).DefaultIfEmpty()
                         where coin is null || (coin.BalEvalAmt > 0 &&
                                                coin.BalEvalAmt < TradingConstants.MinBidAmount)
                         select new { symbol = bid, coin = coin }).ToArray();

            List<OrderPostParameterDto> orders = new List<OrderPostParameterDto>();

            double amount;

            foreach (var item in items)
            {
                var symbol = item.symbol;
                var coin = item.coin;

                if (coin is PositionDto)
                {
                    amount = BidAmount - coin.BalEvalAmt;
                    amount = amount < TradingConstants.MinOrderableAmount ? TradingConstants.MinOrderableAmount : amount;
                }
                else
                {
                    amount = BidAmount;
                }

                if (!TradingTerms.Rebalancing)
                {
                    if (Convert.ToDouble(Cash?.balance) < amount) { continue; }
                }

                orders.Add(new OrderPostParameterDto()
                {
                    side = OrderSide.bid,
                    market = item.symbol,
                    ord_type = OrderType.price,
                    price = Convert.ToDecimal(amount),
                    Remark = OrderReason.Signal.ToDescriptionString()
                });
            }

            return orders;
        }

        private (IEnumerable<OrderPostParameterDto>, IEnumerable<OrderPostParameterDto>) GetRebalancingOders(IEnumerable<OrderPostParameterDto> asks, IEnumerable<OrderPostParameterDto> bids)
        {
            List<OrderPostParameterDto> rebalancings = new();

            var available = Convert.ToDouble(Cash?.balance) + asks.Sum(f => f.BalEvalAmt);

            var amount = Convert.ToDouble(bids.Sum(f => f.price));

            if (available > amount)
            {
                return (rebalancings, bids);
            }

            var necessary = amount - available;

            // 금일 매수종목 제외
            DateTime utc = DateTime.UtcNow.Date;
            var todays = ThisWeekOrders.Where(f => f.created_at.ToUniversalTime().Date == utc.Date &&
                                                   f.side == OrderSide.bid);

            var limit = TradingConstants.MinOrderableAmount * 1.5F;

            var sellables = (from coin in Coins.Where(f => limit < f.BalEvalAmt && f.locked == 0)
                             from order in todays.Where(f => f.market.Equals(coin.market)).DefaultIfEmpty()
                             from ask in asks.Where(f => f.market.Equals(coin.market)).DefaultIfEmpty()
                             where order is null && ask is null
                             select coin).ToArray();

            if (!sellables.Any())
            {
                return (rebalancings, rebalancings);
            }

            if (TradingTerms.RebalancingOrder == OrderBy.asc)
            {
                sellables = sellables.OrderBy(f => f.PnLRat).ToArray();
            }
            else
            {
                sellables = sellables.OrderByDescending(f => f.PnLRat).ToArray();
            }

            // 분담금
            var minimum = TradingConstants.MinOrderableAmount * 1.03F;
            var apportionment = Math.Ceiling(necessary / sellables.Count());

            apportionment = apportionment < minimum ? minimum : apportionment;

            int takes = (int)Math.Ceiling(necessary / apportionment);

            takes = takes < 1 ? 1 : takes;

            decimal balance;

            foreach (var item in sellables.Take(takes))
            {
                balance = (decimal)Math.Round(apportionment / item.trade_price, 8);

                rebalancings.Add(new OrderPostParameterDto()
                {
                    side = OrderSide.ask,
                    market = item.market,
                    ord_type = OrderType.market,
                    volume = balance,
                    BalEvalAmt = item.BalEvalAmt,
                    Remark = $"{OrderReason.Rebalancing.ToDescriptionString()}, {item.PnLRat:N2}%",
                });
            }

            return (rebalancings, bids);
        }

        private void AdjustOriginalBidOrders()
        {
            DateTime now = DateTime.Now;

            var items = (from order in OriginalBidOrders.Where(f => now.Subtract(f.created_at).TotalSeconds > DelayPositionCheckTime)
                         from coin in Coins.Where(f => f.market.Equals(order.market)).DefaultIfEmpty()
                         where coin is not null
                         select order).ToArray();

            foreach (var item in items)
            {
                OriginalBidOrders.Remove(item);
            }
        }

        private bool AdjustBidAmount()
        {
            bool applyMarket = TradingTerms.ApplyMarketPrice;

            if (TradingTerms.AmountOption == BidAmountOption.Fixed)
            {
                BidAmount = TradingTerms.Amount;
            }
            else
            {
                var count = Signals.Count();
                var total = Coins.Sum(f => f.BalEvalAmt) + Convert.ToDouble(Cash?.total_balance);

                if (total == 0) { return false; }
                if (TradingTerms.AmountOption == BidAmountOption.Auto && count == 0) { return false; }

                // 회원등급별 최대 운용자산 맞춤
                total = Math.Min(total, TradingTerms.MaximumAsset);

                int roundDown = TradingConstants.RoundDown; // 절사 단위
                int maxBidAmount = TradingConstants.MaxBidAmount; // 업비트 최대 주문 금액: 1,000,000,000

                float rate = TradingTerms.AmountOption == BidAmountOption.Auto ?
                             1F / count : TradingTerms.AmountRate / 100F;

                BidAmount = (int)(Math.Truncate((total * rate) / roundDown)) * roundDown;  // 절사 단위로 거래

                if (BidAmount < TradingTerms.Minimum)
                {
                    BidAmount = TradingTerms.Minimum;
                }
                else if (TradingTerms.Maximum < BidAmount)
                {
                    BidAmount = TradingTerms.Maximum == 0 ? BidAmount : TradingTerms.Maximum;
                    BidAmount = BidAmount > maxBidAmount ? maxBidAmount : BidAmount;
                }
            }

            return true;
        }

        private bool IsCanProcess()
        {
            if (Depot.Positions is null || TradingTerms is null || UPbitKey is null || Signals is null) { return false; }
            if (!TradingTerms.AutoTrading) { return false; }

            return true;
        }

        public async Task DoTradeAsync()
        {
            if (!IsCanProcess()) { return; }

            try
            {
                if (!AdjustBidAmount()) { return; }

                AdjustOriginalBidOrders();

                var asks = GetAskOrders();
                var bids = GetBidOrders(asks);

                List<OrderPostParameterDto> orders = new List<OrderPostParameterDto>();

                orders.AddRange(asks);

                if (TradingTerms.Rebalancing)
                {
                    (IEnumerable<OrderPostParameterDto> asks, IEnumerable<OrderPostParameterDto> bids) rebalancing = GetRebalancingOders(asks, bids);

                    orders.AddRange(rebalancing.asks);
                    orders.AddRange(rebalancing.bids);
                }
                else
                {
                    orders.AddRange(bids);
                }

                if (!orders.Any()) { return; }

                while (orders.Any())
                {
                    orders = (await DoTradeAsync(orders)).ToList();

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private async Task<IEnumerable<OrderPostParameterDto>> DoTradeAsync(IEnumerable<OrderPostParameterDto> orders)
        {
            List<OrderPostParameterDto> temporaryErrors = new List<OrderPostParameterDto>();

            try
            {
                StringBuilder builder = new StringBuilder();

                ExOrderPost exOrderPost = new ExOrderPost();
                ExOrderPost.ExParameter parameter = null;
                IResult<Order> response = null;

                Order result;
                string message;

                foreach (var order in orders)
                {
                    parameter = _mapper.Map<ExOrderPost.ExParameter>(order);
#if DEBUG
                    response = await Result<Order>.SuccessAsync(new Order(), "Test");
#else
                    response = await exOrderPost.OrderPostAsync(parameter);
#endif
                    result = response.Data;

                    builder.Clear();
                    builder.AppendLine("\n-----------------------------------------------");
                    builder.AppendLine($"\t마켓코드: {order.market}");
                    builder.AppendLine($"\t주문종류: {order.side.ToDescriptionString()}");
                    builder.AppendLine($"\t주문방식: {order.ord_type.ToDescriptionString()}");
                    builder.AppendLine(order.side == OrderSide.ask ?
                                       $"\t주문수량: {order.volume:N8}" :
                                       $"\t주문금액: {order.price:N0}");

                    if (response.Succeeded)
                    {
                        if (result.side == OrderSide.bid)
                        {
                            builder.AppendLine($"\t예치금액: {result.locked:N4}");
                        }

                        if (result.avg_price > 0)
                        {
                            builder.AppendLine($"\t평균단가: {GetPriceText(result.avg_price)}");
                        }

                        if (result.executed_volume > 0)
                        {
                            builder.AppendLine($"\t체결수량: {result.executed_volume:N8}");
                        }

                        if (result.remaining_volume > 0)
                        {
                            builder.AppendLine($"\t잔여수량: {result.remaining_volume:N8}");
                        }

                        if (result.side == OrderSide.bid)
                        {
                            OriginalBidOrders.Add(result);
                        }
                    }

                    message = $"[{response.Code}]:{response.FullMessage}";

                    if (response.Code?.Equals("too_many_request_order") == true ||
                        response.FullMessage?.Contains("일시적인 거래량 급증으로 먼저 접수된 주문을 처리중입니다.") == true)
                    {
                        temporaryErrors.Add(order);
                    }

                    builder.AppendLine($"\t주문상태: {(response.Succeeded ? result.state.ToDescriptionString() : message)}");

#if DEBUG
                    builder.AppendLine($"\t비    고: {order.Remark} {response.FullMessage}");
#else
                    builder.AppendLine($"\t비    고: {order.Remark}");
#endif

                    _logger.LogInformation(builder.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return temporaryErrors;
        }

        private string GetPriceText(double price)
        {
            if (0 < price && price < 1)
            {
                return $"{price:N4}";
            }
            else if (1 < price && price < 100)
            {
                return $"{price:N2}";
            }
            else
            {
                return $"{price:N0}";
            }
        }
    }
}
