using Dreamrosia.Koin.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "주가정보")]
    public class CandleDto
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
        [JsonIgnore]
        public string unit_currency => Symbol.GetUnitCurrency(market);
        [JsonIgnore]
        public string code => Symbol.GetCode(market);
    }
}