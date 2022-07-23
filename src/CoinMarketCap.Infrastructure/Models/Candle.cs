using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Models
{
    [Display(Name = "캔들")]
    public class Candle
    {
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double marketCap { get; set; }
        public DateTime timestamp { get; set; }
    }

    [Display(Name = "캔들 목록")]
    public class Candles
    {
        [JsonProperty("data")]
        public Data List { get; set; }

        public class Data
        {
            public int id { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }

            [JsonProperty("quotes")]
            public IEnumerable<Quote> Quetes { get; set; }
        }

        [Display(Name = "캔들")]
        public class Quote
        {
            [JsonProperty("quote")]
            public Candle Candle { get; set; }
        }
    }
}
