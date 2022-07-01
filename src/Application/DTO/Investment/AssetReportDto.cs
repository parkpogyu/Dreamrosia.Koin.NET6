using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class AssetReportDto : AssetDto
    {
        public double MinMDDDssAmt { get; set; }
        public double MaxMDDDssAmt { get; set; }
        public double AvgMDDDssAmt { get; set; }
        public float MinMDDDssAmtRat { get; set; }
        public float MaxMDDDssAmtRat { get; set; }
        public float AvgMDDDssAmtRat { get; set; }
        public double MinMDDInvsPnL { get; set; }
        public double MaxMDDInvsPnL { get; set; }
        public double AvgMDDInvsPnL { get; set; }
        public float MinMDDInvsPnLRat { get; set; }
        public float MaxMDDInvsPnLRat { get; set; }
        public float AvgMDDInvsPnLRat { get; set; }
        public double MinInvsPnL { get; set; }
        public double AvgInvsPnL { get; set; }
        public float MinInvsPnLRat { get; set; }
        public float MaxInvsPnLRat { get; set; }
        public float AvgInvsPnLRat { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }
        [JsonIgnore]
        public override double PnL => Profit + Loss;
        public double MinProfit { get; set; }
        public double MaxProfit { get; set; }
        public double AvgProfit { get; set; }
        public double MinLoss { get; set; }
        public double MaxLoss { get; set; }
        public double AvgLoss { get; set; }
        public int BidCount { get; set; }
        public int AskCount { get; set; }
        public int ProfitCount { get; set; }
        public int LossCount { get; set; }
        [JsonIgnore]
        public float PnLRatio => AvgLoss == 0 ? 0 : (float)Math.Abs(AvgProfit / AvgLoss);
        [JsonIgnore]
        public float WinningRate => AskCount == 0 ? 0 : (float)Ratio.ToPercentage(ProfitCount, AskCount);
        public DateTime? HeadDate { get; set; }
        public DateTime RearDate { get; set; }

        [JsonIgnore]
        public TimeSpan? TradingTerm
        {
            get
            {
                if (HeadDate is DateTime)
                {
                    return RearDate.Subtract(Convert.ToDateTime(HeadDate));
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public string Duration
        {
            get
            {
                if (HeadDate is DateTime)
                {
                    TimeSpan span = RearDate.Subtract(Convert.ToDateTime(HeadDate));
                    DateTime duration = DateTime.MinValue + span;

                    int yy = duration.Year - 1;
                    int mm = duration.Month - 1;
                    int dd = duration.Day - 1;
                    int days = (int)span.TotalDays;

                    string left = string.Empty;

                    if (yy > 0)
                    {
                        left = $"{yy:D2},{mm:D2},{dd:D2}";
                    }
                    else if (mm > 0)
                    {
                        left = $"{mm:D2},{dd:D2}";
                    }
                    else
                    {
                        left = $"{dd:D2}";
                    }

                    return $"{left} / ({days:N0})";
                }
                else
                {
                    return null;
                }
            }
        }

        public IEnumerable<AssetDto> Assets { get; set; } = new List<AssetDto>();

        public void MakeReport(IEnumerable<PaperOrderDto> orders)
        {
            Deposit = Assets.Any() ? Assets.Last().Deposit : 0;
            BalEvalAmt = Assets.Any() ? Assets.Last().BalEvalAmt : 0;
            InAmt = Assets.Any() ? Assets.Sum(f => f.InAmt) : 0;
            OutAmt = Assets.Any() ? Assets.Sum(f => f.OutAmt) : 0;
            BorrowedAmt = Assets.Any() ? Assets.Max(f => f.BorrowedAmt) : 0;

            if (!orders.Any()) { return; }

            HeadDate = orders.Any() ? orders.Min(f => f.created_at) : null;

            var profit = orders.Where(f => f.PnL > 0);

            Profit = profit.Any() ? profit.Sum(f => f.PnL) : 0;
            MinProfit = profit.Any() ? profit.Min(f => f.PnL) : 0;
            MaxProfit = profit.Any() ? profit.Max(f => f.PnL) : 0;
            AvgProfit = profit.Any() ? profit.Average(f => f.PnL) : 0;
            ProfitCount = profit.Count();

            var loss = orders.Where(f => f.PnL < 0);

            Loss = loss.Any() ? loss.Sum(f => f.PnL) : 0;
            MinLoss = loss.Any() ? loss.Max(f => f.PnL) : 0;
            MaxLoss = loss.Any() ? loss.Min(f => f.PnL) : 0;
            AvgLoss = loss.Any() ? loss.Average(f => f.PnL) : 0;
            LossCount = loss.Count();

            Fee = orders.Any() ? (double)orders.Sum(f => f.paid_fee) : 0;
            MaxDssAmt = Assets.Any() ? Assets.Max(f => f.MaxDssAmt) : 0;

            var mdds = Assets.Where(f => f.MDDDssAmt < 0).ToArray();

            MinMDDDssAmt = mdds.Any() ? mdds.Max(f => f.MDDDssAmt) : 0;
            MaxMDDDssAmt = mdds.Any() ? mdds.Min(f => f.MDDDssAmt) : 0;
            AvgMDDDssAmt = mdds.Any() ? mdds.Select(f => f.MDDDssAmt).Distinct().Average() : 0;

            MinMDDDssAmtRat = mdds.Any() ? mdds.Max(f => f.MDDDssAmtRat) : 0;
            MaxMDDDssAmtRat = mdds.Any() ? mdds.Min(f => f.MDDDssAmtRat) : 0;
            AvgMDDDssAmtRat = mdds.Any() ? mdds.Select(f => f.MDDDssAmtRat).Distinct().Average() : 0;

            var mddPnLs = Assets.Where(f => f.MDDInvsPnL < 0).ToArray();

            MinMDDInvsPnL = mddPnLs.Any() ? mddPnLs.Max(f => f.MDDInvsPnL) : 0;
            MaxMDDInvsPnL = mddPnLs.Any() ? mddPnLs.Min(f => f.MDDInvsPnL) : 0;
            AvgMDDInvsPnL = mddPnLs.Any() ? mddPnLs.Select(f => f.MDDInvsPnL).Distinct().Average() : 0;

            MinMDDInvsPnLRat = mddPnLs.Any() ? mddPnLs.Max(f => f.MDDInvsPnLRat) : 0;
            MaxMDDInvsPnLRat = mddPnLs.Any() ? mddPnLs.Min(f => f.MDDInvsPnLRat) : 0;
            AvgMDDInvsPnLRat = mddPnLs.Any() ? mddPnLs.Select(f => f.MDDInvsPnLRat).Distinct().Average() : 0;

            var invsPnls = Assets.Where(f => f.InvsPnL != 0).ToArray();

            MinInvsPnL = invsPnls.Any() ? invsPnls.Min(f => f.InvsPnL) : 0;
            MaxInvsPnL = invsPnls.Any() ? invsPnls.Max(f => f.InvsPnL) : 0;
            AvgInvsPnL = invsPnls.Any() ? invsPnls.Select(f => f.InvsPnL).Distinct().Average() : 0;

            MinInvsPnLRat = invsPnls.Any() ? invsPnls.Min(f => f.InvsPnLRat) : 0;
            MaxInvsPnLRat = invsPnls.Any() ? invsPnls.Max(f => f.InvsPnLRat) : 0;
            AvgInvsPnLRat = invsPnls.Any() ? invsPnls.Select(f => f.InvsPnLRat).Distinct().Average() : 0;

            BidCount = orders.Any() ? orders.Where(f => f.side == OrderSide.bid).Count() : 0;
            AskCount = orders.Any() ? orders.Where(f => f.side == OrderSide.ask).Count() : 0;
        }
    }
}
