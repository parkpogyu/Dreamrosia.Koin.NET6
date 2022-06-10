using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "체결정보")]
    public class MarketTrade
    {
        [Display(Name = "마켓코드")]
        public string market { get; set; }

        [Display(Name = "체결일자(UTC)")]
        public DateTime trade_date_utc { get; set; }

        [Display(Name = "체결시간(UTC)")]
        public DateTime trade_time_utc { get; set; }

        [Display(Name = "체결시간")]
        public long timestamp { get; set; }

        [Display(Name = "체결가격")]
        public double trade_price { get; set; }

        [Display(Name = "체결수량")]
        public double trade_volume { get; set; }

        [Display(Name = "전일종가")]
        public double prev_closing_price { get; set; }

        [Display(Name = "이전대비")]
        public double chane_price { get; set; }

        [Display(Name = "거래종류")]
        public OrderSide ask_bid { get; set; }

        [Display(Name = "체결번호")]
        public long sequential_id { get; set; }
    }
}
