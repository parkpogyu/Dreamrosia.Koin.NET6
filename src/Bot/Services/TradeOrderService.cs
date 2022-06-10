using AutoMapper;
using Dreamrosia.Koin.Bot.Constants;
using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Extentions;
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

        private readonly List<OrderPostParameterDto> OriginalBidOrders = new List<OrderPostParameterDto>();

        private double BidAmount { get; set; }

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
            var sellables = Coins.Where(f => OrderPostParameterDto.MinimumOrderableAmount < f.BalEvalAmt &&
                                             0 < f.balance).ToArray();

            List<OrderPostParameterDto> orders = new List<OrderPostParameterDto>();

            if (TradingTerms.LiquidatePositions)
            {
                foreach (var position in sellables)
                {
                    orders.Add(new OrderPostParameterDto()
                    {
                        side = OrderSide.ask,
                        market = position.market,
                        ord_type = OrderType.market,
                        volume = position.balance,
                        Remark = string.Format("{0}, {1:N2}", OrderReason.Liquidate.ToDescriptionString(), position.PnLRat),
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
                    Remark = string.Format("{0}, {1:N2}", OrderReason.Signal.ToDescriptionString(), item.PnLRat),
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
                        Remark = string.Format("{0}, {1:N2}", reason.ToDescriptionString(), item.PnLRat),
                    });
                }
            }

            OrderReason GetCutOffReason(PositionDto item)
            {
                OrderReason reason = OrderReason.None;

                if (TradingTerms.UseStopLoss && -100D < item.PnLRat && item.PnLRat < TradingTerms.StopLoss)
                {
                    reason = OrderReason.StopLoss;
                }
                else if (TradingTerms.UseTakeProfit && TradingTerms.TakeProfit < item.PnLRat && double.IsFinite(item.PnLRat))
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
                                                coin.BalEvalAmt < OrderPostParameterDto.MinimumBidAmount)
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
                }
                else
                {
                    amount = BidAmount;
                }

                if (Cash?.balance < amount || amount < OrderPostParameterDto.MinimumOrderableAmount) { continue; }

                orders.Add(new OrderPostParameterDto()
                {
                    side = OrderSide.bid,
                    market = item.symbol,
                    ord_type = OrderType.price,
                    price = amount,
                    Remark = OrderReason.Signal.ToDescriptionString()
                }); ;
            }

            OriginalBidOrders.AddRange(orders);

            return orders;
        }

        private void AdjustOriginalBidOrders()
        {
            var items = (from order in OriginalBidOrders
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
                var total = Coins.Sum(f => f.BalEvalAmt) + Cash?.balance;

                if (total is null) { return false; }

#if DEBUG
                // 회원등급별 최대 운용자산 맞춤
                total = Math.Min(Convert.ToDouble(total), TradingTerms.MaximumAsset);
#endif

                double roundDown = TradingConstants.RoundDownUnit; // 절사 단위
                double maxBidAmount = TradingConstants.MaxBidAmount; // KRW 최대 주문 금액: 1,000,000,000

                TradingTerms.AmountRate = TradingTerms.AmountOption == BidAmountOption.Auto ? 100F / Signals.Count() : TradingTerms.AmountRate;

                BidAmount = (long)(total * (TradingTerms.AmountRate / 100F));
                BidAmount = (BidAmount / roundDown) * roundDown;  // 천원 단위로 거래

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
                orders.AddRange(bids);

                if (!orders.Any()) { return; }

                StringBuilder builder = new StringBuilder();

                ExOrderPost exOrderPost = new ExOrderPost();
                ExOrderPost.ExParameter parameter = null;
                IResult<Order> result = null;

                foreach (var order in orders)
                {
                    parameter = _mapper.Map<ExOrderPost.ExParameter>(order);
#if DEBUG
                    result = await Result<Order>.SuccessAsync(new Order(), "Test");
#else
                    result = await exOrderPost.OrderPostAsync(parameter);
#endif
                    builder.Clear();
                    builder.AppendLine("\n-----------------------------------------------");
                    builder.AppendLine(string.Format("\t마켓코드: {0}", order.market));
                    builder.AppendLine(string.Format("\t주문종류: {0}", order.side.ToDescriptionString()));
                    builder.AppendLine(string.Format("\t주문방식: {0}", order.ord_type.ToDescriptionString()));
                    builder.AppendLine(order.side == OrderSide.ask ?
                                       string.Format("\t주문수량: {0:N8}", order.volume) :
                                       string.Format("\t주문금액: {0:N0}", order.price));
                    builder.AppendLine(string.Format("\t주문결과: {0}", result.Succeeded ? "완료" : result.Messages.First()));
                    builder.AppendLine(string.Format("\t비    고: {0}", order.Remark));

                    _logger.LogInformation(builder.ToString());

                    if (result.Succeeded) { continue; }

                    var failed = OriginalBidOrders.SingleOrDefault(f => f.market.Equals(order.market));

                    if (failed is OrderPostParameterDto)
                    {
                        OriginalBidOrders.Remove(failed);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
