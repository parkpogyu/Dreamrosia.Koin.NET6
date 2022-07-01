using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "현재가 정보")]
    public class Ticker
    {
        // 타입   필드                   설명
        //-------------------------------------------------------------------------------------
        // String market                종목 구분 코드    
        // String trade_date            최근 거래 일자(UTC)   
        // String trade_time            최근 거래 시각(UTC)  
        // String trade_date_kst        최근 거래 일자(KST) 
        // String trade_time_kst        최근 거래 시각(KST)   
        // Double opening_price         시가 
        // Double high_price            고가
        // Double low_price             저가 
        // Double trade_price           종가(현재가) 
        // Double prev_closing_price    전일 종가(UTC 0시 기준)    
        // String change                EVEN : 보합
        //                              RISE : 상승
        //                              FALL : 하락 
        // Double change_price          변화액의 절대값 
        // Double change_rate           변화율의 절대값 
        // Double signed_change_price   부호가 있는 변화액  
        // Double signed_change_rate    부호가 있는 변화율 
        // Double trade_volume          가장 최근 거래량   
        // Double acc_trade_price       누적 거래대금(UTC 0시 기준)  
        // Double acc_trade_price_24h	24시간 누적 거래대금 
        // Double acc_trade_volume      누적 거래량(UTC 0시 기준)   
        // Double acc_trade_volume_24h	24시간 누적 거래량 
        // Double highest_52_week_price	52주 신고가 
        // String highest_52_week_date	52주 신고가 달성일 
        // Double lowest_52_week_price	52주 신저가 
        // String lowest_52_week_date	52주 신저가 달성일 
        // Long timestamp               타임스탬프   Long
        //-------------------------------------------------------------------------------------

        public string market { get; set; }
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

        public static double PriceUnit(double price)
        {
            // =====================================================
            // 최소 호가        | 최대 호가     | 주문 가격 단위(원)
            // =====================================================
            // 2,000,000        |               | 1,000
            // -----------------------------------------------------
            // 1,000,000        | 2,000,000     | 500
            // -----------------------------------------------------
            // 500,000          | 1,000,000     | 100
            // -----------------------------------------------------
            // 100,000          | 500,000       | 50
            // -----------------------------------------------------
            // 10,000           | 100,000       | 10
            // -----------------------------------------------------
            // 1,000            | 10,000        | 5
            // -----------------------------------------------------
            // 100              | 1,000         | 1
            // -----------------------------------------------------
            // 10               | 100           | 0.1
            // -----------------------------------------------------
            // 0                | 10            | 0.01
            // =====================================================
            if (2000000 < price)
            {
                return 1000;
            }
            else if (1000000 < price && price <= 2000000)
            {
                return 500;
            }
            else if (500000 < price && price <= 1000000)
            {
                return 100;
            }
            else if (100000 < price && price <= 500000)
            {
                return 50;
            }
            else if (10000 < price && price <= 100000)
            {
                return 10;
            }
            else if (1000 < price && price <= 10000)
            {
                return 5;
            }
            else if (100 < price && price <= 1000)
            {
                return 1;
            }
            else if (10 < price && price <= 100)
            {
                return 0.1;
            }
            else // ( 0 < pr && pr <= 10)
            {
                return 0.01;
            }
        }
    }
}
