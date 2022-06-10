using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "심볼정보")]
    public class CrixDto
    {
        public string crix_code { get; set; }

        public string koreanName { get; set; }

        public string englishName { get; set; }

        public string code { get; set; }

        public string unit_currency { get; set; }

        public double price { get; set; }

        public double? marketCap { get; set; }

        public double accTradePrice24h { get; set; }

        public double signedChangeRate1h { get; set; }

        public double signedChangeRate24h { get; set; }

        public double availableVolume { get; set; }

        public string provider { get; set; }

        public DateTime lastUpdated { get; set; }

        public long timestamp { get; set; }
    }
}