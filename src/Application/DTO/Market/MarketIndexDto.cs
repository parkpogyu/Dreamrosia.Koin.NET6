using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class MarketIndexDto
    {
        public string code { get; set; }
        public DateTime candleDateTimeUtc { get; set; }
        public DateTime candleDateTimeKst { get; set; }
        public double openingPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double tradePrice { get; set; }
        public double prevClosingPrice { get; set; }
        public double signedChangePrice { get; set; }
        public double signedChangeRate { get; set; }
    }
}
