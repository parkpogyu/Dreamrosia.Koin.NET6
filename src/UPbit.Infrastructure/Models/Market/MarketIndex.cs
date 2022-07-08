using Newtonsoft.Json;
using System;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    public class MarketIndex
    {
        [JsonProperty("candleDateTime")]
        public DateTime candleDateTimeUtc { get; set; }
        public DateTime candleDateTimeKst { get; set; }
        public double openingPrice { get; set; }
        public double highPrice { get; set; }
        public double lowPrice { get; set; }
        public double tradePrice { get; set; }
        //public double candleAccTradeVolume { get; set; }
        //public double candleAccTradePrice { get; set; }
        //public long? timestamp { get; set; }
        public string code { get; set; }
        public double prevClosingPrice { get; set; }
        //public string change { get; set; }
        //public double changePrice { get; set; }
        public double signedChangePrice { get; set; }
        //public double changeRate { get; set; }
        public double signedChangeRate { get; set; }
    }
}
