using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "심볼정보")]
    public class SymbolDto
    {
        public string market { get; set; }

        public string korean_name { get; set; }

        public string english_name { get; set; }

        public MarketAlert market_warning { get; set; } = MarketAlert.None;

        public SeasonSignals DailySignal { get; set; } = SeasonSignals.Indeterminate;

        public SeasonSignals WeeklySignal { get; set; } = SeasonSignals.Indeterminate;

        /// <summary>
        /// 기준화폐
        /// </summary>
        [JsonIgnore]
        public string unit_currency => Symbol.GetUnitCurrency(market);

        /// <summary>
        /// 화폐코드
        /// </summary>
        [JsonIgnore]
        public string code => Symbol.GetCode(market);

        /// <summary>
        /// 현재가
        /// </summary>
        public double? trade_price { get; set; }

        /// 시가총액(억)
        public double? marketCap { get; set; }

        // 거래대금(억)
        public double accTradePrice24h { get; set; }

        public double signed_change_rate { get; set; }
    }
}