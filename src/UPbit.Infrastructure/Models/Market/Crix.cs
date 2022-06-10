using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "Cryptocurrency Index(CRIX)")]
    public class Crix
    {
        [JsonProperty("code")]
        public string crix_code { get; set; }

        public string koreanName { get; set; }

        public string englishName { get; set; }

        [JsonProperty("symbol")]
        public string code { get; set; }

        [JsonProperty("currencyCode")]
        public string unit_currency { get; set; }

        public double price { get; set; }

        public double? marketCap { get; set; }

        public double accTradePrice24h { get; set; }

        public double signedChangeRate1h { get; set; }

        public double signedChangeRate24h { get; set; }

        public double? availableVolume { get; set; }

        public string provider { get; set; }

        public DateTime lastUpdated { get; set; } // 2021-05-30T12:18:02+09:00

        public long timestamp { get; set; }
    }
}
