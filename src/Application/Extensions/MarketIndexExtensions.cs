using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class MarketIndexExtensions
    {
        public static IEnumerable<MarketIndexDto> GetTimeFrameMarketIndices(this IEnumerable<MarketIndexDto> sources, TimeFrames frame = TimeFrames.Day, bool firstDayOfPeriod = true)
        {
            if (!sources.Any()) { return sources; }

            var _sources = sources.OrderBy(f => f.candleDateTimeUtc);

            if (frame == TimeFrames.Day)
            {
                return _sources.ToArray();
            }
            else
            {
                List<List<MarketIndexDto>> items = new List<List<MarketIndexDto>>();
                List<MarketIndexDto> indices = new List<MarketIndexDto>();

                items.Add(indices);

                DateTime first_day_of_period = _sources.Min(f => f.candleDateTimeUtc);

                if (frame == TimeFrames.Week)
                {
                    foreach (var index in _sources)
                    {
                        if (index.candleDateTimeUtc.DayOfWeek == DayOfWeek.Monday)
                        {
                            first_day_of_period = index.candleDateTimeUtc;

                            indices = new List<MarketIndexDto>();

                            items.Add(indices);
                        }

                        indices.Add(index);
                    }
                }
                else if (frame == TimeFrames.Month)
                {
                    foreach (var index in _sources)
                    {
                        if (index.candleDateTimeUtc.Day == 1)
                        {
                            first_day_of_period = index.candleDateTimeUtc;

                            indices = new List<MarketIndexDto>();

                            items.Add(indices);
                        }

                        indices.Add(index);
                    }
                }
                else if (frame == TimeFrames.Year)
                {
                    var year = first_day_of_period.Year;

                    foreach (var index in _sources)
                    {
                        if (index.candleDateTimeUtc.Year != year)
                        {
                            first_day_of_period = index.candleDateTimeUtc;

                            year = first_day_of_period.Year;

                            indices = new List<MarketIndexDto>();

                            items.Add(indices);
                        }

                        indices.Add(index);
                    }
                }

                return items.Where(f => f.Any())
                            .Select(f =>
                            {
                                var head = f.First();
                                var rear = f.Last();

                                MarketIndexDto group = new MarketIndexDto()
                                {
                                    candleDateTimeUtc = firstDayOfPeriod ? head.candleDateTimeUtc : rear.candleDateTimeUtc,
                                    tradePrice = rear.tradePrice,
                                };

                                return group;
                            }).ToArray();
            }
        }
    }
}
