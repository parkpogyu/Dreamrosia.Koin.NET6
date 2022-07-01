using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Extensions;
using Dreamrosia.Koin.Shared.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class BackTestingTraderService : IBackTestingTraderService
    {
        private readonly ICandleService _candleService;
        private readonly IMACDService _macdService;
        private readonly ILogger<AssetService> _logger;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;


        public BackTestingTraderService(ICandleService candleService,
                                        IMACDService macdService,
                                        ILogger<AssetService> logger,
                                        IMapper mapper,
                                        IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _candleService = candleService;
            _macdService = macdService;
            _logger = logger;
            _mapper = mapper;
            _localizer = localizer;
        }

        private IEnumerable<MacdContainer> WeeklyContainers { get; set; }
        private IEnumerable<MacdContainer> DailyContainers { get; set; }

        private SeasonSignals WeeklySignal { get; set; } = SeasonSignals.Indeterminate;

        public DateTime GetFirstSignalDate(TimeFrames frame)
        {
            MacdContainer container = null;

            if (frame == TimeFrames.Day)
            {
                if (DailyContainers is null) { return DateTime.UtcNow.Date; }

                container = DailyContainers.FirstOrDefault(f => f.SeasonSignals == SeasonSignals.GoldenCross);
            }
            else if (frame == TimeFrames.Week)
            {
                if (WeeklyContainers is null) { return DateTime.UtcNow.Date; }

                container = WeeklyContainers.FirstOrDefault(f => f.SeasonSignals == SeasonSignals.GoldenCross);
            }

            return container is null ? DateTime.UtcNow.Date : container.Source.candle_date_time_utc;
        }

        private IEnumerable<CandleDto> candles { get; set; }

        public PaperPositionDto Position { get; private set; }

        public List<PaperPositionDto> Positions { get; private set; } = new List<PaperPositionDto>();

        public List<PaperOrderDto> Orders { get; private set; } = new List<PaperOrderDto>();

        private string market { get; set; }

        private TradingTermsDto terms { get; set; }

        public async Task Prepare(BackTestingRequestDto model, string market, DateTime head, DateTime rear)
        {
            this.market = market;
            this.terms = model;

            candles = (await _candleService.GetCandlesAsync(market, head, rear)).Data.Reverse();

            DailyContainers = _macdService.Generate(candles).Reverse().ToArray();

            var limit = DateTime.UtcNow.FirstDayOfWeek(DayOfWeek.Monday).ToUniversalDate();

            var weekly = candles.GetTimeFrameCandles(TimeFrames.Week, firstDayOfPeriod: false)
                                .Where(f => f.candle_date_time_utc < limit)
                                .ToArray();

            WeeklyContainers = _macdService.Generate(weekly).Reverse();

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

                                Position.balance = order.executed_volume;
                                Position.avg_buy_price = order.avg_price;
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

                    decimal balance = Position.balance + order.executed_volume;
                    Position.avg_buy_price = (Position.PchsAmt + order.exec_amount) / (double)balance;
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

                Position.balance = order.executed_volume;
                Position.avg_buy_price = order.avg_price;
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
            order.executed_volume = (decimal)(terms.Amount / next.opening_price);
            order.exec_amount = order.avg_price * (double)order.executed_volume;
            order.paid_fee = (decimal)(order.exec_amount * DefaultValue.Fees.Rate4KRW);
            order.VolumeRate = (float)Ratio.ToPercentage((double)next.candle_acc_trade_volume, (double)order.executed_volume);

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
            order.exec_amount = order.avg_price * (double)order.executed_volume;
            order.paid_fee = (decimal)(order.exec_amount * DefaultValue.Fees.Rate4KRW);
            order.PnL = (double)Position.balance * (order.avg_price - Position.avg_buy_price);
            order.PnLRat = (float)Ratio.ToSignedPercentage(order.avg_price, Position.avg_buy_price);
            order.VolumeRate = (float)Ratio.ToPercentage(next.candle_acc_trade_volume, (double)order.executed_volume);

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
                    var downRate = (float)Ratio.ToSignedPercentage(Position.trade_price, Position.high_price);

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