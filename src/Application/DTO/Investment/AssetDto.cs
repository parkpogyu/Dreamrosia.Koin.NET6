using Dreamrosia.Koin.Shared.Common;
using System;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class AssetDto
    {
        public int PositionCount { get; set; }
        public double AskAmt { get; set; }
        public double BidAmt { get; set; }
        public double Fee { get; set; }
        public virtual double PnL { get; set; }
        public double BalEvalAmt { get; set; }
        public double InAmt { get; set; }
        public double OutAmt { get; set; }
        public double BorrowedAmt { get; set; }
        public double Deposit { get; set; }
        [JsonIgnore]
        public double DssAmt => Deposit + BalEvalAmt;
        public double InvsAmt { get; set; }
        [JsonIgnore]
        public double NecessaryAmt => InvsAmt + BorrowedAmt;
        [JsonIgnore]
        public double InvsPnL => DssAmt - NecessaryAmt;
        [JsonIgnore]
        public virtual float InvsPnLRat => NecessaryAmt == 0 ? 0 : (float)Ratio.ToSignedPercentage(DssAmt, NecessaryAmt);
        [JsonIgnore]
        public double MDDDssAmt => MaxDssAmt == 0 ? 0 : DssAmt - MaxDssAmt;
        [JsonIgnore]
        public float MDDDssAmtRat => MaxDssAmt == 0 ? 0 : (float)Ratio.ToSignedPercentage(DssAmt, MaxDssAmt);
        public double MaxDssAmt { get; set; }
        public double MaxInvsPnL { get; set; }
        [JsonIgnore]
        public double MDDInvsPnL => MaxInvsPnL == 0 ? 0 : InvsPnL - MaxInvsPnL;
        [JsonIgnore]
        public float MDDInvsPnLRat => MaxInvsPnL == 0 ? 0 : (float)Ratio.ToSignedPercentage(InvsPnL, MaxInvsPnL);

        public DateTime created_at { get; set; }
    }
}
