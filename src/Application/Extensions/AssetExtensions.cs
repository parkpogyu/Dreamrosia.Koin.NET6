using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class AssetExtensions
    {
        public static IEnumerable<AssetExtensionDto> GetTimeFrameAssets(this IEnumerable<AssetExtensionDto> sources, TimeFrames frame = TimeFrames.Day, bool firstDayOfPeriod = true)
        {
            if (!sources.Any()) { return sources; }

            var _sources = sources.OrderBy(f => f.created_at);

            if (frame == TimeFrames.Day)
            {
                return _sources.ToArray();
            }
            else
            {
                List<List<AssetExtensionDto>> items = new List<List<AssetExtensionDto>>();
                List<AssetExtensionDto> assets = new List<AssetExtensionDto>();

                items.Add(assets);

                DateTime first_day_of_period = _sources.Min(f => f.created_at);

                if (frame == TimeFrames.Week)
                {
                    foreach (var asset in _sources)
                    {
                        if (asset.created_at.DayOfWeek == DayOfWeek.Monday)
                        {
                            first_day_of_period = asset.created_at;

                            assets = new List<AssetExtensionDto>();

                            items.Add(assets);
                        }

                        assets.Add(asset);
                    }
                }
                else if (frame == TimeFrames.Month)
                {
                    foreach (var asset in _sources)
                    {
                        if (asset.created_at.Day == 1)
                        {
                            first_day_of_period = asset.created_at;

                            assets = new List<AssetExtensionDto>();

                            items.Add(assets);
                        }

                        assets.Add(asset);
                    }
                }
                else if (frame == TimeFrames.Year)
                {
                    var year = first_day_of_period.Year;

                    foreach (var asset in _sources)
                    {
                        if (asset.created_at.Year != year)
                        {
                            first_day_of_period = asset.created_at;

                            year = first_day_of_period.Year;

                            assets = new List<AssetExtensionDto>();

                            items.Add(assets);
                        }

                        assets.Add(asset);
                    }
                }

                return items.Where(f => f.Any())
                            .Select(f =>
                            {
                                var head = f.First();
                                var rear = f.Last();

                                AssetExtensionDto group = new AssetExtensionDto()
                                {
                                    BalEvalAmt = rear.BalEvalAmt,
                                    Deposit = rear.Deposit,
                                    InvsAmt = rear.InvsAmt,
                                    BorrowedAmt = rear.BorrowedAmt,
                                    InAmt = f.Sum(f => f.InAmt),
                                    OutAmt = f.Sum(f => f.OutAmt),
                                    AskAmt = f.Sum(f => f.AskAmt),
                                    BidAmt = f.Sum(f => f.BidAmt),
                                    Fee = f.Sum(f => f.Fee),
                                    PnL = f.Sum(f => f.PnL),
                                    MaxDssAmt = f.Max(f => f.MaxDssAmt),
                                    MaxInvsPnL = f.Max(f => f.MaxInvsPnL),
                                    PositionCount = f.Sum(f => f.PositionCount),
                                    created_at = rear.created_at,
                                    index = rear.index,
                                };

                                return group;
                            }).ToArray();
            }
        }
    }
}
