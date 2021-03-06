using Dreamrosia.Koin.Shared.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "보유코인")]
    public class PositionDto
    {
        public string UserId { get; set; }

        public string code { get; set; }
        public decimal balance { get; set; }
        public decimal locked { get; set; }
        public double avg_buy_price { get; set; }
        public bool avg_buy_price_modified { get; set; }
        public string unit_currency { get; set; }
        public bool IsListed { get; set; }

        [JsonIgnore]
        public string market => $"{unit_currency}-{code}";
        public double trade_price { get; set; }
        public double high_price { get; set; }
        [JsonIgnore]
        public decimal total_balance => balance + locked;
        [JsonIgnore]
        public double PchsAmt => avg_buy_price * (double)total_balance;
        [JsonIgnore]
        public double BalEvalAmt => trade_price * (double)total_balance;
        [JsonIgnore]
        public double EvalPnL => BalEvalAmt - PchsAmt;
        [JsonIgnore]
        public float PnLRat => avg_buy_price == 0 ? 0 : (float)Ratio.ToSignedPercentage(trade_price, avg_buy_price);
    }
}