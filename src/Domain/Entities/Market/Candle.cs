using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Candle : AuditableEntity<int>
    {
        /// <summary>
        /// 마켓코드
        /// </summary>
        //[Required]
        //[StringLength(20)]
        public string market { get; set; }

        /// <summary>
        /// 기준일시 (UTC)
        /// </summary>
        //[Required]
        public DateTime candle_date_time_utc { get; set; }

        /// <summary>
        /// 기준일시 (한국)
        /// </summary>
        //[Required]
        public DateTime candle_date_time_kst { get; set; }

        /// <summary>
        /// 시가
        /// </summary>
        //[Required]
        public double opening_price { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        //[Required]
        public double high_price { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        //[Required]
        public double low_price { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        //[Required]
        public double trade_price { get; set; }

        /// <summary>
        /// 갱신일시
        /// </summary>
        public long? timestamp { get; set; }

        /// <summary>
        /// 누적 거래대금 
        /// </summary>
        public double candle_acc_trade_price { get; set; }

        /// <summary>
        /// 누적 거래량
        /// </summary>
        public double candle_acc_trade_volume { get; set; }
    }
}
