using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "캔들")]
    public class Candle
    {
        [Display(Name = "마켓코드")]
        public string market { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime candle_date_time_utc { get; set; }

        [Display(Name = "기준시간(KST)")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime candle_date_time_kst { get; set; }

        [Display(Name = "시가")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double opening_price { get; set; }

        [Display(Name = "고가")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double high_price { get; set; }

        [Display(Name = "저가")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double low_price { get; set; }

        [Display(Name = "종가")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double trade_price { get; set; }

        [Display(Name = "최종 틱 시간")]
        public long? timestamp { get; set; }

        [Display(Name = "거래대금")]
        public double candle_acc_trade_price { get; set; }

        [Display(Name = "거래량")]
        public double candle_acc_trade_volume { get; set; }

        #region Minutes
        [Display(Name = "분 단위")]
        public int? unit { get; set; }
        #endregion

        #region Day
        [Display(Name = "전일종가")]
        public double? prev_closing_price { get; set; }

        [Display(Name = "전일대비")]
        public double? change_price { get; set; }

        [Display(Name = "등락률")]
        public double? change_rate { get; set; }

        [Display(Name = "환산가격")]
        public double? converted_trade_price { get; set; }
        #endregion

        #region Week, Month
        [Display(Name = "시작일자")]
        public DateTime? first_day_of_period { get; set; }
        #endregion
    }
}
