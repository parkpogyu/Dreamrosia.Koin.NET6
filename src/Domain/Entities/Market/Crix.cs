using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class Crix : AuditableEntity<string>
    {
        public string koreanName { get; set; }
        public string englishName { get; set; }
        public string code { get; set; }
        public string unit_currency { get; set; }
        public decimal price { get; set; }
        public decimal marketCap { get; set; }
        public decimal accTradePrice24h { get; set; }
        public decimal availableVolume { get; set; }
        public string provider { get; set; }
        public DateTime lastUpdated { get; set; } // 2021-05-30T12:18:02+09:00
        public long timestamp { get; set; }
    }
}
