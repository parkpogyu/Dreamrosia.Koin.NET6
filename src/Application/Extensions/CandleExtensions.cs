using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class CandleExtensions
    {
        public static IEnumerable<CandleDto> GetTimeFrameCandles(this IEnumerable<CandleDto> sources, TimeFrames frame = TimeFrames.Day, bool firstDayOfPeriod = true)
        {
            if (!sources.Any()) { return sources; }

            var _sources = sources.OrderBy(f => f.candle_date_time_utc);

            if (frame == TimeFrames.Day)
            {
                return _sources.ToArray();
            }
            else
            {
                List<List<CandleDto>> items = new List<List<CandleDto>>();
                List<CandleDto> candles = new List<CandleDto>();

                items.Add(candles);

                DateTime first_day_of_period = _sources.Min(f => f.candle_date_time_utc);

                if (frame == TimeFrames.Week)
                {
                    foreach (var candle in _sources)
                    {
                        if (candle.candle_date_time_utc.DayOfWeek == DayOfWeek.Monday)
                        {
                            first_day_of_period = candle.candle_date_time_utc;

                            candles = new List<CandleDto>();

                            items.Add(candles);
                        }

                        candles.Add(candle);
                    }
                }
                else if (frame == TimeFrames.Month)
                {
                    foreach (var candle in _sources)
                    {
                        if (candle.candle_date_time_utc.Day == 1)
                        {
                            first_day_of_period = candle.candle_date_time_utc;

                            candles = new List<CandleDto>();

                            items.Add(candles);
                        }

                        candles.Add(candle);
                    }
                }
                else if (frame == TimeFrames.Year)
                {
                    var year = first_day_of_period.Year;

                    foreach (var candle in _sources)
                    {
                        if (candle.candle_date_time_utc.Year != year)
                        {
                            first_day_of_period = candle.candle_date_time_utc;

                            year = first_day_of_period.Year;

                            candles = new List<CandleDto>();

                            items.Add(candles);
                        }

                        candles.Add(candle);
                    }
                }

                return items.Where(f => f.Any())
                            .Select(f =>
                            {
                                var head = f.First();
                                var rear = f.Last();

                                CandleDto group = new CandleDto()
                                {
                                    market = head.market,
                                    candle_date_time_utc = firstDayOfPeriod ? head.candle_date_time_utc : rear.candle_date_time_utc,
                                    candle_date_time_kst = firstDayOfPeriod ? head.candle_date_time_kst : rear.candle_date_time_kst,
                                    opening_price = head.opening_price,
                                    high_price = f.Max(m => m.high_price),
                                    low_price = f.Min(m => m.low_price),
                                    trade_price = rear.trade_price,
                                    candle_acc_trade_price = f.Sum(f => f.candle_acc_trade_price),
                                    candle_acc_trade_volume = f.Sum(f => f.candle_acc_trade_volume),
                                };

                                return group;
                            }).ToArray();
            }
        }
    }
}
