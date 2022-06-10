using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "마켓정보")]
    public class Market
    {
        [Display(Name = "아이디")]
        public string id { get; set; }

        [Display(Name = "이름")]
        public string name { get; set; }

        [Display(Name = "지원주문방식")]
        public List<OrderType> order_types { get; set; }

        [Display(Name = "지원주문종류")]
        public List<OrderSide> order_sides { get; set; }

        [Display(Name = "제약사항(매수)")]
        public Restrictions bid { get; set; }

        [Display(Name = "제약사항(매도)")]
        public Restrictions ask { get; set; }

        [Display(Name = "최대 거래금액")]
        public double max_total { get; set; }

        [Display(Name = "운영상태")]
        public string state { get; set; }
    }
}
