using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "호가정보")]
    public class OrderBook
    {
        [Display(Name = "마켓코드")]
        public string market { get; set; }

        [Display(Name = "호가생성시간")]
        public long timestamp { get; set; }

        [Display(Name = "매도총잔량")]
        public double total_ask_size { get; set; }

        [Display(Name = "매수총잔량")]
        public double total_bid_size { get; set; }

        [Display(Name = "호가목록")]
        public List<OrderBookUnit> orderbook_units { get; set; }

        public class OrderBookUnit
        {
            [Display(Name = "매도호가")]
            public double ask_price { get; set; }

            [Display(Name = "매수호가")]
            public double bid_price { get; set; }

            [Display(Name = "매도잔량")]
            public double ask_size { get; set; }

            [Display(Name = "매수잔량")]
            public double bid_size { get; set; }
        }

        public class Unit
        {
            [Display(Name = "매수잔량")]
            public double? bid_size { get; set; }

            [Display(Name = "호가")]
            public double price { get; set; }

            [Display(Name = "매도잔량")]
            public double? ask_size { get; set; }

            public OrderSide Side => bid_size != null ? OrderSide.bid : OrderSide.ask;

            public static string SizeFormatString(double size)
            {
                return 10000 < size ? "{0:N0}" : "{0:N4}";
            }
        }
    }
}
