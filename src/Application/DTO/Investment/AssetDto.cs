using Dreamrosia.Koin.Shared.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class AssetDto
    {
        [Display(Name = "보유코인", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int PositionCount { get; set; }

        [Display(Name = "매도금액", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double AskAmt { get; set; }

        [Display(Name = "매수금액", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double BidAmt { get; set; }

        [Display(Name = "수수료", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Fee { get; set; }

        [Display(Name = "실현손익", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public virtual double PnL { get; set; }

        [Display(Name = "평가금액", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double BalEvalAmt { get; set; }

        [Display(Name = "입금액", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double InAmt { get; set; }

        [Display(Name = "출금액", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double OutAmt { get; set; }

        [Display(Name = "차용금", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double BorrowedAmt { get; set; }

        [Display(Name = "예수금", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Deposit { get; set; }

        [JsonIgnore]
        [Display(Name = "총 자산", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double DssAmt => Deposit + BalEvalAmt;

        [Display(Name = "투자자금", GroupName = "자산정보")]
        public double InvsAmt { get; set; }

        [JsonIgnore]
        [Display(Name = "필요자금", GroupName = "자산정보")]
        public double NecessaryAmt => InvsAmt + BorrowedAmt;

        [JsonIgnore]
        [Display(Name = "투자손익", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double InvsPnL => DssAmt - NecessaryAmt;

        [JsonIgnore]
        [Display(Name = "투자손익률(%)", GroupName = "자산정보")]
        public virtual double InvsPnLRat => NecessaryAmt == 0 ? 0 : Ratio.ToSignedPercentage(DssAmt, NecessaryAmt);

        [JsonIgnore]
        [Display(Name = "보유자산 하락폭", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double MDDDssAmt => MaxDssAmt == 0 ? 0 : DssAmt - MaxDssAmt;

        [JsonIgnore]
        [Display(Name = "보유자산 하락률(%)", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MDDDssAmtRat => MaxDssAmt == 0 ? 0 : Ratio.ToSignedPercentage(DssAmt, MaxDssAmt);

        [Display(Name = "최대 보유자산", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MaxDssAmt { get; set; }

        [Display(Name = "최대 투자손익", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MaxInvsPnL { get; set; }

        [JsonIgnore]
        [Display(Name = "투자손익 하락폭", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MDDInvsPnL => MaxInvsPnL == 0 ? 0 : InvsPnL - MaxInvsPnL;

        [JsonIgnore]
        [Display(Name = "투자손익 하락률(%)", GroupName = "자산정보")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double MDDInvsPnLRat => MaxInvsPnL == 0 ? 0 : Ratio.ToSignedPercentage(InvsPnL, MaxInvsPnL);

        [Display(Name = "작성일자")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime created_at { get; set; }
    }
}
