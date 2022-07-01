using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "보유코인")]
    public class PositionDto : Position
    {
        public string UserId { get; set; }

        public string market => $"{unit_currency}-{code}";
        public double trade_price { get; set; }
        public decimal total_balance => balance + locked;
        public double PchsAmt => avg_buy_price * (double)total_balance;
        public double BalEvalAmt => trade_price * (double)total_balance;
        public double EvalPnL => BalEvalAmt - PchsAmt;
        public float PnLRat => PchsAmt == 0 ? 0F : (float)(EvalPnL / PchsAmt) * 100F;
    }
}