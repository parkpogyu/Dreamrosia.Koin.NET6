using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Models
{
    [Display(Name = "심볼정보")]
    public class Symbol
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
    }

    [Display(Name = "심볼정보 목록")]
    public class Symbols
    {
        [JsonProperty("data")]
        public Data List { get; set; }

        public class Data
        {
            [JsonProperty("cryptoCurrencyList")]
            public IEnumerable<Symbol> Symbols { get; set; }

            public string totalCount { get; set; }
        }
    }
}
