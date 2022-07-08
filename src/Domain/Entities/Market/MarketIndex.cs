using Dreamrosia.Koin.Domain.Contracts;
using System;

namespace Dreamrosia.Koin.Domain.Entities
{
    public class MarketIndex : AuditableEntity<int>
    {
        public DateTime candleDateTimeUtc { get; set; }
        public DateTime candleDateTimeKst { get; set; }
        public double openingPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double tradePrice { get; set; }
        public string code { get; set; }
        public double prevClosingPrice { get; set; }
        public double signedChangePrice { get; set; }
        public double signedChangeRate { get; set; }
    }
}
