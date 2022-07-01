using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "체결정보")]
    public class MarketTrade
    {
        // 타입   필드                   설명
        //-------------------------------------------------------------------------------------
        // String trade_date_utc        체결 일자(UTC 기준)   
        // String trade_time_utc        체결 시각(UTC 기준)  
        // Long   timestamp             체결 타임스탬프    
        // Double trade_price           체결 가격   
        // Double trade_volume          체결량 
        // Double prev_closing_price    전일 종가(UTC 0시 기준)    
        // Double change_price          변화량 
        // String ask_bid               매도/매수 
        // Long   sequential_id         체결 번호(Unique)   
        //-------------------------------------------------------------------------------------

        public string market { get; set; }
        public DateTime trade_date_utc { get; set; }
        public DateTime trade_time_utc { get; set; }
        public long timestamp { get; set; }
        public decimal trade_price { get; set; }
        public decimal trade_volume { get; set; }
        public decimal prev_closing_price { get; set; }
        public decimal chane_price { get; set; }
        public OrderSide ask_bid { get; set; }
        public long sequential_id { get; set; }
    }
}
