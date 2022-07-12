using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class MarketIndexDto
    {
        public DateTime candleDateTimeUtc { get; set; }
        public double openingPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double tradePrice { get; set; }
    }
}
