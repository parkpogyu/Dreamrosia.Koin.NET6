using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Bot.DTO
{
    [Display(Name = "보유코인")]
    public class PositionDto
    {
        public string UserId { get; set; }

        public string code { get; set; }

        public double balance { get; set; }

        public double locked { get; set; }

        public double avg_buy_price { get; set; }

        public bool avg_buy_price_modified { get; set; }

        public string unit_currency { get; set; }

        public string market => $"{unit_currency}-{code}";

        public double trade_price { get; set; }

        public double total_balance => balance + locked;

        public double PchsAmt => avg_buy_price * total_balance;

        public double BalEvalAmt => trade_price * total_balance;

        public double EvalPnL => BalEvalAmt - PchsAmt;

        public double PnLRat
        {
            get
            {
                // 코인 최소가격: .0001
                if (avg_buy_price == 0 || trade_price < .0001)
                {
                    return 0D;
                }
                else
                {
                    return ((trade_price / avg_buy_price) - 1D) * 100D;
                }
            }
        }
    }
}