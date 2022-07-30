using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Candle : AuditableEntity<int>
    {
        public string market { get; set; }

        public DateTime candle_date_time_utc { get; set; }
        public DateTime candle_date_time_kst { get; set; }
        public double opening_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public double trade_price { get; set; }
        public double candle_acc_trade_price { get; set; }
        public double candle_acc_trade_volume { get; set; }
    }

    public class OldCandle : AuditableEntity<int>
    {
        public string market { get; set; }

        public DateTime candle_date_time_utc { get; set; }
        public DateTime candle_date_time_kst { get; set; }
        public double opening_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public double trade_price { get; set; }
        public double candle_acc_trade_price { get; set; }
        public double candle_acc_trade_volume { get; set; }
    }
}
