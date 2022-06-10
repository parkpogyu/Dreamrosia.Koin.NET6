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
        /// <summary>
        /// 마켓코드
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 시가
        /// </summary>
        public double opening_price { get; set; }

        /// <summary>
        /// 고가
        /// </summary>
        public double high_price { get; set; }

        /// <summary>
        /// 저가
        /// </summary>
        public double low_price { get; set; }

        /// <summary>
        /// 현재가
        /// </summary>
        public double trade_price { get; set; }

        /// <summary>
        /// 전일 종가
        /// </summary>
        public double prev_closing_price { get; set; }

        /// <summary>
        /// 전일 대비
        /// </summary>
        public TickerDirection change { get; set; }

        /// <summary>
        /// 전일대비 절대값
        /// </summary>
        public double change_price { get; set; }

        /// <summary>
        /// 전일대비 등락률 절대값
        /// </summary>
        public double change_rate { get; set; }

        /// <summary>
        /// 전일대비
        /// </summary>
        public double signed_change_price { get; set; }

        /// <summary>
        /// 전일대비 등락률
        /// </summary>
        public double signed_change_rate { get; set; }

        /// <summary>
        /// 최근 거래량 
        /// </summary>
        public double trade_volume { get; set; }

        /// <summary>
        /// 누적 거래량(UTC 0시 기준)
        /// </summary>
        public double acc_trade_volume { get; set; }

        /// <summary>
        /// 누적 거개량(24H)
        /// </summary>
        public double acc_trade_volume_24h { get; set; }

        /// <summary>
        /// 누적 거래대금(UTC 0시 기준)
        /// </summary>
        public double acc_trade_price { get; set; }

        /// <summary>
        /// 누적 거래대금(24H)
        /// </summary>
        public double acc_trade_price_24h { get; set; }

        /// <summary>
        /// 최근 거래일자(UTC)
        /// </summary>
        public string trade_date { get; set; }

        /// <summary>
        /// 최근 거래시각(UTC)
        /// </summary>
        public string trade_time { get; set; }

        /// <summary>
        /// 체결 타임스탬프
        /// </summary>
        public long trade_timestamp { get; set; }

        /// <summary>
        /// 매수/매도 구분
        /// </summary>
        public OrderSide ask_bid { get; set; }

        /// <summary>
        /// 누적 매도량
        /// </summary>
        public double acc_ask_volume { get; set; }

        /// <summary>
        /// 누적 매수량
        /// </summary>
        public double acc_bid_volumne { get; set; }

        /// <summary>
        /// 52주 신고가
        /// </summary>
        public double highest_52_week_price { get; set; }

        /// <summary>
        /// 52주 신고가일
        /// </summary>
        public DateTime highest_52_week_date { get; set; }

        /// <summary>
        /// 52주 신저가
        /// </summary>
        public double lowest_52_week_price { get; set; }

        /// <summary>
        /// 52주 신저가일
        /// </summary>
        public DateTime lowest_52_week_date { get; set; }

        /// <summary>
        /// 거래상태
        /// </summary>
        public string trade_status { get; set; }

        /// <summary>
        /// 거래상태
        /// </summary>
        public string market_state { get; set; }

        /// <summary>
        /// 거래정지 여부
        /// </summary>
        public bool is_trading_suspended { get; set; }

        /// <summary>
        /// 유의종목 여부
        /// </summary>
        public string market_warning { get; set; }


        public long timestamp { get; set; }

        /// <summary>
        /// 스트림 타입
        /// </summary>
        public string stream_type { get; set; }
    }
}
