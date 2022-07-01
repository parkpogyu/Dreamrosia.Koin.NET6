using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "캔들")]
    public class Candle
    {
        // 타입     필드                    설명
        //-------------------------------------------------------------------------------------
        // String   market                  마켓명 
        // String   candle_date_time_utc    캔들 기준 시각(UTC 기준)
        // String   candle_date_time_kst    캔들 기준 시각(KST 기준)
        // Double   opening_price           시가
        // Double   high_price              고가
        // Double   low_price               저가
        // Double   trade_price             종가
        // Long     timestamp               해당 캔들에서 마지막 틱이 저장된 시각 
        // Double   candle_acc_trade_price  누적 거래 금액 
        // Double   candle_acc_trade_volume 누적 거래량 
        //-------------------------------------------------------------------------------------
        // Integer  unit                    분 단위(유닛)
        //-------------------------------------------------------------------------------------
        // Double   prev_closing_price      전일 종가(UTC 0시 기준) 
        // Double   change_price            전일 종가 대비 변화 금액 
        // Double   change_rate             전일 종가 대비 변화량 
        // Double   converted_trade_price   종가 환산 화폐 단위로 환산된 가격
        //                                  (요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.)   
        //-------------------------------------------------------------------------------------
        // String   first_day_of_period     캔들 기간의 가장 첫 날
        //-------------------------------------------------------------------------------------

        public string market { get; set; }
        public DateTime candle_date_time_utc { get; set; }
        public DateTime candle_date_time_kst { get; set; }
        public double opening_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public double trade_price { get; set; }
        public long? timestamp { get; set; }
        public double candle_acc_trade_price { get; set; }
        public double candle_acc_trade_volume { get; set; }

        #region Minutes
        public int unit { get; set; }
        #endregion

        #region Day
        public double? prev_closing_price { get; set; }
        public double change_price { get; set; }
        public double change_rate { get; set; }
        public double converted_trade_price { get; set; }
        #endregion

        #region Week, Month
        public DateTime? first_day_of_period { get; set; }
        #endregion
    }
}
