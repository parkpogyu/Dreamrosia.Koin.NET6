using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    public class OrderAvailable
    {
        [Display(Name = "매수 수수료")]
        public double bid_fee { get; set; }

        [Display(Name = "매수 수수료율")]
        public double bid_fee_rate => bid_fee * 100D;

        [Display(Name = "매도 수수료")]
        public double ask_fee { get; set; }

        [Display(Name = "매도 수수료율")]
        public double ask_fee_rate => ask_fee * 100D;

        [Display(Name = "마켓정보")]
        public Market market { get; set; }

        [Display(Name = "매수용 계좌상태")]
        public Position bid_account { get; set; }

        [Display(Name = "매도용 계좌상태")]
        public Position ask_account { get; set; }
    }
}
