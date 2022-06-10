using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class AssetReportDto : AssetDto
    {
        [Display(Name = "보유자산 최소 하락폭)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinMDDDssAmt { get; set; }

        [Display(Name = "보유자산 최대 하락폭")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxMDDDssAmt { get; set; }

        [Display(Name = "보유자산 평균 하락폭)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgMDDDssAmt { get; set; }

        [Display(Name = "보유자산 최소 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinMDDDssAmtRat { get; set; }

        [Display(Name = "보유자산 최대 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxMDDDssAmtRat { get; set; }

        [Display(Name = "보유자산 평균 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgMDDDssAmtRat { get; set; }

        [Display(Name = "투자손익 최소 하락폭)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinMDDInvsPnL { get; set; }

        [Display(Name = "투자손익 최대 하락폭")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxMDDInvsPnL { get; set; }

        [Display(Name = "투자손익 평균 하락폭)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgMDDInvsPnL { get; set; }

        [Display(Name = "투자손익 최소 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinMDDInvsPnLRat { get; set; }

        [Display(Name = "투자손익 최대 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxMDDInvsPnLRat { get; set; }

        [Display(Name = "투자손익 평균 하략률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgMDDInvsPnLRat { get; set; }

        [Display(Name = "최소 투자손익")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MinInvsPnL { get; set; }

        [Display(Name = "평균 투자손익)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgInvsPnL { get; set; }

        [Display(Name = "최소 투자손익률 (%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinInvsPnLRat { get; set; }

        [Display(Name = "최대 투자손익률 (%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxInvsPnLRat { get; set; }

        [Display(Name = "평균 투자손익률 (%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgInvsPnLRat { get; set; }

        [Display(Name = "매도 총 수익")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Profit { get; set; }

        [Display(Name = "매도 총 손실")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Loss { get; set; }

        [JsonIgnore]
        public override double PnL => Profit + Loss;

        [Display(Name = "매도 최소 수익")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinProfit { get; set; }

        [Display(Name = "매도 최대 수익")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxProfit { get; set; }

        [Display(Name = "매도 평균 수익")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgProfit { get; set; }

        [Display(Name = "매도 최소 손실")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MinLoss { get; set; }

        [Display(Name = "매도 최대 손실")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MaxLoss { get; set; }

        [Display(Name = "매도 평균 손실")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AvgLoss { get; set; }

        [Display(Name = "매수 건수")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double BidCount { get; set; }

        [Display(Name = "매도 건수")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AskCount { get; set; }

        [Display(Name = "수익 건수")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double ProfitCount { get; set; }

        [Display(Name = "손실 건수")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double LossCount { get; set; }

        [JsonIgnore]
        [Display(Name = "손익비")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double PnLRatio => AvgLoss == 0 ? 0 : Math.Abs(AvgProfit / AvgLoss);

        [JsonIgnore]
        [Display(Name = "승률(%)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double WinningRate => AskCount == 0 ? 0 : Ratio.ToPercentage(ProfitCount, AskCount);

        [Display(Name = "시작일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? HeadDate { get; set; }

        [Display(Name = "종료일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
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
                        left = $"{yy:D2}-{mm:D2}-{dd:D2}";
                    }
                    else if (mm > 0)
                    {
                        left = $"{mm:D2}-{dd:D2}";
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

            Fee = orders.Any() ? orders.Sum(f => Convert.ToDouble(f.paid_fee)) : 0;
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
