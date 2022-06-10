using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Indicators;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Extensions;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class MockTradingService : IMockTradingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICandleService _candleService;
        private readonly ISymbolService _symbolService;
        private readonly ILogger<AssetService> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public MockTradingService(IServiceProvider serviceProvider,
                                  ICandleService candleService,
                                  ISymbolService symbolService,
                                  ILogger<AssetService> logger,
                                  IMapper mapper,
                                  IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _serviceProvider = serviceProvider;
            _candleService = candleService;
            _symbolService = symbolService;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IResult<byte[]>> GetBackTestingAsync(BackTestRequestDto model)
        {
            var markets = (await _symbolService.GetSymbolCodesAsync()).Data;

            IEnumerable<string> candidates = null;

            candidates = (from lt in markets
                          from rt in model.ChosenSymbols.Where(f => f.Equals(lt)).DefaultIfEmpty()
                          where rt is not null
                          select lt).ToArray();

            candidates = candidates.Any() ? candidates : markets;

            var first = new DateTime().ToUniversalDate();
            var today = DateTime.UtcNow.Date;

            List<MockTrader> traders = new List<MockTrader>();
            foreach (var code in candidates)
            {
                var candles = (await _candleService.GetCandlesAsync(code,
                                                                    first,
                                                                    today)).Data;

                var trader = new MockTrader(code, model, candles.OrderBy(f => f.candle_date_time_utc), _logger, _mapper);

                trader.Prepare();

                traders.Add(trader);
            }

            var head = traders.Min(f => f.FirstSignalDate);
            var rear = Convert.ToDateTime(model.RearDate).Date;
            var startDate = Convert.ToDateTime(model.HeadDate).Date;

            head = head > startDate ? head : startDate;

            var dates = DateTimeExtensions.GetDays(head, (int)rear.Subtract(head).TotalDays + 1);
            var count = candidates.Count();

            List<AssetDto> assets = new List<AssetDto>();

            AssetDto asset = new AssetDto()
            {
                Deposit = model.SeedMoney,
                InvsAmt = model.SeedMoney,
            };

            foreach (var date in dates)
            {
                model.Amount = (long)AdjustBidAmount(asset.DssAmt, count);

                Parallel.ForEach(traders, trader =>
                {
                    trader.Simulate(date, model.TimeFrame);
                });

                asset = MakeAsset(date, traders, asset);

                assets.Add(asset);
            }

            BackTestReportDto report = new BackTestReportDto();

            report.Assets = assets;
            report.InvsAmt = model.SeedMoney;

            var orders = traders.SelectMany(f => f.Orders).OrderBy(f => f.created_at);

            if (orders.Any())
            {
                report.MakeReport(orders);
            }
            else
            {
                report.HeadDate = dates.First().Date;
            }

            report.RearDate = dates.Last();

            report.Orders = model.IncludeOrders ? orders : new List<PaperOrderDto>();
            report.Positions = model.IncludePositons ? traders.SelectMany(f => f.Positions.Where(f => f.balance > 0))
                                                              .OrderBy(f => f.created_at) : new List<PaperPositionDto>();

            if (report.Positions.Any())
            {
                foreach (var position in report.Positions)
                {
                    position.IsListed = true;
                }
            }

            var compressed = await ObjectGZip.CompressAsync(report);

            return await Result<byte[]>.SuccessAsync(compressed);

            double AdjustBidAmount(double total, int count)
            {
                bool applyMarket = model.ApplyMarketPrice;

                double amount = 0;

                if (model.AmountOption == BidAmountOption.Fixed)
                {
                    amount = model.Amount;
                }
                else
                {
                    double cutOffUnit = 1000D; // 절사 단위
                    double MaxBidAmount = 1000000000D; // KRW 최대 주문 금액: 1,000,000,000

                    model.AmountRate = model.AmountOption == BidAmountOption.Auto ? 100F / count : model.AmountRate;

                    amount = (long)(total * (model.AmountRate / 100F));
                    amount = (amount / cutOffUnit) * cutOffUnit;  // 천원 단위로 거래

                    if (amount < model.Minimum)
                    {
                        amount = model.Minimum;
                    }
                    else if (model.Maximum < amount)
                    {
                        amount = model.Maximum == 0 ? amount : model.Maximum;
                        amount = amount > MaxBidAmount ? MaxBidAmount : amount;
                    }
                }

                return amount;
            }

            AssetDto MakeAsset(DateTime date, IEnumerable<MockTrader> traders, AssetDto prev)
            {
                var positions = traders.Where(f => f.Position.created_at == date && f.Position.balance > 0)
                                       .Select(f => f.Position)
                                       .ToArray();

                var orders = traders.SelectMany(f => f.Orders.Where(o => o.created_at == date)).ToArray();

                var bids = orders.Where(f => f.side == OrderSide.bid).ToArray();
                var asks = orders.Where(f => f.side == OrderSide.ask).ToArray();

                AssetDto asset = new AssetDto()
                {
                    InvsAmt = prev.InvsAmt,
                    created_at = date,
                };

                asset.AskAmt = asks.Sum(f => f.exec_amount);
                asset.BidAmt = bids.Sum(f => f.exec_amount);
                asset.Fee = asks.Sum(f => Convert.ToDouble(f.paid_fee)) + bids.Sum(f => Convert.ToDouble(f.paid_fee));
                asset.PnL = asks.Sum(f => f.PnL);
                asset.BalEvalAmt = positions.Sum(f => f.BalEvalAmt);
                asset.Deposit = prev.Deposit - (asset.BidAmt + asset.Fee) + asset.AskAmt;

                if (asset.Deposit < 0)
                {
                    asset.InAmt = Math.Abs(asset.Deposit);
                    asset.Deposit = 0;

                    asset.BorrowedAmt = prev.BorrowedAmt + asset.InAmt;
                }
                else if (0 < prev.BorrowedAmt && prev.BorrowedAmt < asset.Deposit)
                {
                    asset.OutAmt = prev.BorrowedAmt;
                    asset.Deposit = asset.Deposit - prev.BorrowedAmt;
                    asset.BorrowedAmt = 0;
                }
                else
                {
                    asset.BorrowedAmt = prev.BorrowedAmt;
                }

                asset.PositionCount = positions.Count();

                foreach (var trader in traders)
                {
                    if (trader.Position.IsCleared)
                    {
                        trader.Position.balance = 0;
                    }
                }

                asset.MaxDssAmt = asset.DssAmt > prev.MaxDssAmt ? asset.DssAmt : prev.MaxDssAmt;
                asset.MaxInvsPnL = asset.InvsPnL > prev.MaxInvsPnL ? asset.InvsPnL : prev.MaxInvsPnL;

                return asset;
            }
        }

        private class MockTrader
        {
            private IEnumerable<MovingAverageConvergenceDivergence.Container> WeeklyContainers { get; set; }
            private IEnumerable<MovingAverageConvergenceDivergence.Container> DailyContainers { get; set; }

            private SeasonSignals WeeklySignal { get; set; } = SeasonSignals.Indeterminate;

            private MovingAverageConvergenceDivergence MACD = new MovingAverageConvergenceDivergence()
            {
                Short = 2,
                Long = 10,
            };

            public DateTime FirstSignalDate
            {
                get
                {
                    if (WeeklyContainers is null) { return DateTime.UtcNow.Date; }

                    var goldenCross = WeeklyContainers.FirstOrDefault(f => f.SeasonSignals == SeasonSignals.GoldenCross);

                    return goldenCross is null ? DateTime.UtcNow.Date : goldenCross.Source.candle_date_time_utc;
                }
            }

            private IEnumerable<CandleDto> candles { get; set; }

            public PaperPositionDto Position { get; private set; }

            public List<PaperPositionDto> Positions { get; private set; } = new List<PaperPositionDto>();

            public List<PaperOrderDto> Orders { get; private set; } = new List<PaperOrderDto>();

            private string market;

            private TradingTermsDto terms;

            private ILogger _logger;
            private IMapper _mapper;

            public MockTrader(string market, TradingTermsDto terms, IEnumerable<CandleDto> candles, ILogger logger, IMapper mapper)
            {
                this.market = market;
                this.terms = terms;
                this.candles = candles;

                _logger = logger;
                _mapper = mapper;
            }

            public void Prepare()
            {
                DailyContainers = MACD.Generate(candles).Reverse().ToArray();

                var limit = DateTime.UtcNow.FirstDayOfWeek(DayOfWeek.Monday).ToUniversalDate();

                var weekly = candles.GetTimeFrameCandles(TimeFrames.Week, firstDayOfPeriod: false)
                                    .Where(f => f.candle_date_time_utc < limit)
                                    .ToArray();

                WeeklyContainers = MACD.Generate(weekly).Reverse();

                Position = new PaperPositionDto()
                {
                    code = Symbol.GetCode(market),
                    unit_currency = Currency.Unit.KRW,
                };
            }

            public void Simulate(DateTime date, TimeFrames frame = TimeFrames.Week)
            {
                if (frame == TimeFrames.Day)
                {
                    SimulateDaily(date);
                }
                else if (frame == TimeFrames.Week)
                {
                    SimulateWeekly(date);
                }
            }

            private void SimulateWeekly(DateTime date)
            {
                PaperOrderDto order = null;

                var wcontainer = WeeklyContainers.SingleOrDefault(f => f.Source.candle_date_time_utc == date);
                CandleDto candle = candles.SingleOrDefault(f => f.candle_date_time_utc == date);

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    WeeklySignal = wcontainer is not null ? wcontainer.SeasonSignals : WeeklySignal;
                }

                if (candle is null) { return; }

                if (!Position.IsCleared)
                {
                    order = GetCutOffOrder(candle);
                }

                if (order is null)
                {
                    if (wcontainer is null)
                    {
                        if (Position.IsCleared)
                        {
                            var dcontainer = DailyContainers.SingleOrDefault(f => f.Source.candle_date_time_utc == date);

                            if (dcontainer is not null)
                            {
                                if (WeeklySignal == SeasonSignals.Above && dcontainer.SeasonSignals == SeasonSignals.GoldenCross)
                                {
                                    order = MakeBidOrder(candle, daily: true);

                                    if (order is null) { return; }

                                    Position.balance = Convert.ToDouble(order.executed_volume);
                                    Position.avg_buy_price = Convert.ToDouble(order.avg_price);
                                    Position.high_price = candle.high_price;
                                    Position.IsCleared = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        order = GetTradeOrder(candle, wcontainer.SeasonSignals);
                    }
                }

                if (order is not null)
                {
                    Orders.Add(order);
                }
            }

            private void SimulateDaily(DateTime date)
            {
                PaperOrderDto order = null;

                var dcontainer = DailyContainers.SingleOrDefault(f => f.Source.candle_date_time_utc == date);
                CandleDto candle = candles.SingleOrDefault(f => f.candle_date_time_utc == date);

                if (candle is null || dcontainer is null) { return; }

                if (!Position.IsCleared)
                {
                    order = GetCutOffOrder(candle);
                }

                if (order is null)
                {
                    order = GetTradeOrder(candle, dcontainer.SeasonSignals);
                }

                if (order is not null)
                {
                    Orders.Add(order);
                }
            }

            private PaperOrderDto GetCutOffOrder(CandleDto candle)
            {
                Position.created_at = candle.candle_date_time_utc;
                Position.trade_price = candle.trade_price;
                Position.high_price = Math.Max(Position.high_price, candle.high_price);

                Positions.Add(_mapper.Map<PaperPositionDto>(Position));

                PaperOrderDto order = MakeCutOffOrder(candle);

                if (order is null)
                {
                    if (terms.Pyramiding)
                    {
                        order = MakePyramingOrder(candle);

                        if (order is null) { return order; }

                        double balance = Position.balance + Convert.ToDouble(order.executed_volume);

                        Position.avg_buy_price = (Position.PchsAmt + order.exec_amount) / balance;

                        Position.balance = balance;
                    }
                }
                else
                {
                    Position.IsCleared = true;
                }

                return order;
            }

            private PaperOrderDto GetTradeOrder(CandleDto candle, SeasonSignals signals)
            {
                PaperOrderDto order = null;

                if (signals == SeasonSignals.GoldenCross)
                {
                    order = MakeBidOrder(candle);

                    if (order is null) { return order; }

                    Position.balance = Convert.ToDouble(order.executed_volume);
                    Position.avg_buy_price = Convert.ToDouble(order.avg_price);
                    Position.high_price = candle.high_price;
                    Position.IsCleared = false;
                }
                else if (signals == SeasonSignals.DeadCross)
                {
                    if (Position.balance == 0) { return order; }

                    order = MakeAskOrder(candle);

                    if (order is null) { return order; }

                    Position.created_at = candle.candle_date_time_utc;
                    Position.trade_price = candle.trade_price;
                    Position.high_price = 0;

                    Positions.Add(_mapper.Map<PaperPositionDto>(Position));

                    Position.IsCleared = true;
                }
                else
                {
                    if (Position.IsCleared) { return order; }

                    Position.created_at = candle.candle_date_time_utc;
                    Position.trade_price = candle.trade_price;
                    Position.high_price = Math.Max(Position.high_price, candle.high_price);

                    Positions.Add(_mapper.Map<PaperPositionDto>(Position));
                }

                return order;
            }

            private PaperOrderDto MakeBidOrder(CandleDto candle, bool daily = false)
            {
                CandleDto next = candles.FirstOrDefault(f => f.candle_date_time_utc > candle.candle_date_time_utc);

                if (next is null) { return default; }

                PaperOrderDto order = new PaperOrderDto()
                {
                    created_at = next.candle_date_time_utc,
                    side = OrderSide.bid,
                    market = market,
                    ord_type = OrderType.price,
                    state = OrderState.done,
                    Remark = daily ? TimeFrames.Day.ToDescriptionString() : string.Empty,
                };

                order.avg_price = next.opening_price;
                order.executed_volume = next is null ? null : terms.Amount / (double?)next.opening_price;
                order.exec_amount = order.avg_price * Convert.ToDouble(order.executed_volume);
                order.paid_fee = order.exec_amount * DefaultValue.Fees.Rate4KRW;
                order.VolumeRate = Convert.ToDouble(order.executed_volume) / next.candle_acc_trade_volume * Ratio.Hundreadth;

                return order;
            }

            private PaperOrderDto MakeAskOrder(CandleDto candle)
            {
                CandleDto next = candles.FirstOrDefault(f => f.candle_date_time_utc > candle.candle_date_time_utc);

                if (next is null) { return default; }

                PaperOrderDto order = new PaperOrderDto()
                {
                    created_at = next.candle_date_time_utc,
                    side = OrderSide.ask,
                    market = Position.market,
                    ord_type = OrderType.market,
                    executed_volume = Position.balance,
                    state = OrderState.done,
                };

                order.avg_price = next.opening_price;
                order.exec_amount = order.avg_price * Convert.ToDouble(order.executed_volume);
                order.paid_fee = order.exec_amount * DefaultValue.Fees.Rate4KRW;
                order.PnL = Position.balance * (order.avg_price - Position.avg_buy_price);
                order.PnLRat = (order.avg_price - Position.avg_buy_price) / Position.avg_buy_price * Ratio.Hundreadth;

                order.VolumeRate = Convert.ToDouble(order.executed_volume) / next.candle_acc_trade_volume * Ratio.Hundreadth;

                return order;
            }

            private PaperOrderDto MakeCutOffOrder(CandleDto candle)
            {
                if (Position.balance == 0) { return default; }

                PaperOrderDto order = null;

                if (terms.UseStopLoss && Position.PnLRat < terms.StopLoss)
                {
                    order = MakeAskOrder(candle);

                    order.Remark = OrderReason.StopLoss.ToDescriptionString();
                }
                else if (terms.UseTakeProfit && Position.PnLRat > terms.TakeProfit)
                {
                    order = MakeAskOrder(candle);

                    order.Remark = OrderReason.TakeProfit.ToDescriptionString();
                }
                else if (terms.UseTrailingStop)
                {
                    if (Position.high_price != 0)
                    {
                        var downRate = ((Position.trade_price / Position.high_price) - 1) * 100F;

                        if (downRate <= terms.TrailingStop)
                        {
                            order = MakeAskOrder(candle);

                            order.Remark = OrderReason.TrailingStop.ToDescriptionString();
                        }
                    }
                }

                return order;
            }

            private PaperOrderDto MakePyramingOrder(CandleDto current)
            {
                PaperOrderDto order = null;

                DateTime today = current.candle_date_time_utc;

                if (!(today.GetWeekOfMonth() == 2 && today.DayOfWeek == DayOfWeek.Monday)) { return order; }

                CandleDto prev = candles.LastOrDefault(f => f.candle_date_time_utc < today.AddMonths(-1));

                if (prev is null) { return order; }

                if (prev.trade_price > current.trade_price) { return order; }

                CandleDto next = candles.FirstOrDefault(f => f.candle_date_time_utc > today);

                if (next is null) { return order; }

                order = MakeBidOrder(current);

                order.Remark = "추매";

                return order;
            }
        }
    }
}