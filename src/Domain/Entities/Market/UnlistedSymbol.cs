using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class UnlistedSymbol : AuditableEntity<string>
    {
        public int? CoinMarketCapId { get; set; }
        public string english_name { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public DateTime timestamp { get; set; }
    }
}
