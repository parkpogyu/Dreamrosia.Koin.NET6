using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamrosia.Koin.Domain.Entities
{
    /// <summary>
    /// WebSocket Response
    /// </summary>
    [NotMapped]
    public class Ticker
    {
        public string code { get; set; }
        public string trade_date { get; set; }
        public string trade_time { get; set; }
        public string trade_date_kst { get; set; }
        public string trade_time_kst { get; set; }
        public long trade_timestamp { get; set; }
        public double opening_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public double trade_price { get; set; }
        public double prev_closing_price { get; set; }
        public TickerDirection change { get; set; }
        public double change_price { get; set; }
        public double change_rate { get; set; }
        public double signed_change_price { get; set; }
        public double signed_change_rate { get; set; }
        public double trade_volume { get; set; }
        public double acc_trade_price { get; set; }
        public double acc_trade_price_24h { get; set; }
        public double acc_trade_volume { get; set; }
        public double acc_trade_volume_24h { get; set; }
        public double highest_52_week_price { get; set; }
        public DateTime highest_52_week_date { get; set; }
        public double lowest_52_week_price { get; set; }
        public DateTime lowest_52_week_date { get; set; }
        public long timestamp { get; set; }
    }
}
