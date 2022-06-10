using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "현재가 정보")]
    public class Ticker
    {
        [Display(Name = "마켓코드", GroupName = "시세")]
        public string market { get; set; }

        [Display(Name = "거래일자(UTC)", GroupName = "시세")]
        public string trade_date { get; set; }

        [Display(Name = "거래시간(UTC)", GroupName = "시세")]
        public string trade_time { get; set; }

        [Display(Name = "거래일자(KST)", GroupName = "시세")]
        public string trade_date_kst { get; set; }

        [Display(Name = "거래시간(KST)", GroupName = "시세")]
        public string trade_time_kst { get; set; }

        [Display(Name = "거래시간", GroupName = "시세")]
        public long trade_timestamp { get; set; }

        [Display(Name = "시가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double opening_price { get; set; }

        [Display(Name = "고가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double high_price { get; set; }

        [Display(Name = "저가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double low_price { get; set; }

        [Display(Name = "현재가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double trade_price { get; set; }

        [Display(Name = "전일종가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double prev_closing_price { get; set; }

        [Display(Name = "방향", GroupName = "시세")]
        public TickerDirection change { get; set; }

        [Display(Name = "이전대비", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double change_price { get; set; }

        [Display(Name = "전일대비", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double diff_prev_closing_price => trade_price - prev_closing_price;

        [Display(Name = "등락률(%)", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double change_rate { get; set; }

        [Display(Name = "이전대비", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double signed_change_price { get; set; }

        [Display(Name = "등락률(%)", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double signed_change_rate { get; set; }

        [Display(Name = "최근거래량", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double trade_volume { get; set; }

        [Display(Name = "누적거래대금(UTC 0시 기준)", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double acc_trade_price { get; set; }

        [Display(Name = "누적거래대금(24H))", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double acc_trade_price_24h { get; set; }

        [Display(Name = "누적거래량(UTC 0시 기준)", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double acc_trade_volume { get; set; }

        [Display(Name = "누적거래량(24H))", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double acc_trade_volume_24h { get; set; }

        [Display(Name = "52주 신고가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double highest_52_week_price { get; set; }

        [Display(Name = "52주 신고가일", GroupName = "시세")]
        public DateTime highest_52_week_date { get; set; }

        [Display(Name = "52주 신저가", GroupName = "시세")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double lowest_52_week_price { get; set; }

        [Display(Name = "52주 신저가일", GroupName = "시세")]
        public DateTime lowest_52_week_date { get; set; }

        [Display(Name = "")]
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
