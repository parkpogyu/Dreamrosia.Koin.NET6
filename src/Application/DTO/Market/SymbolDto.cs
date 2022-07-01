using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "심볼정보")]
    public class SymbolDto
    {
        private string _market { get; set; }
        public string market
        {
            get => _market;
            set
            {
                _market = value;

                unit_currency = Symbol.GetUnitCurrency(_market);
                code = Symbol.GetCode(_market);
            }
        }
        public string korean_name { get; set; }
        public string english_name { get; set; }
        public MarketAlert market_warning { get; set; } = MarketAlert.None;
        public SeasonSignals DailySignal { get; set; } = SeasonSignals.Indeterminate;
        public SeasonSignals WeeklySignal { get; set; } = SeasonSignals.Indeterminate;

        [JsonIgnore]
        public string unit_currency { get; private set; }

        [JsonIgnore]
        public string code { get; private set; }

        public double trade_price { get; set; }
        public double marketCap { get; set; }
        public double accTradePrice24h { get; set; }
        public double signed_change_rate { get; set; }
    }
}