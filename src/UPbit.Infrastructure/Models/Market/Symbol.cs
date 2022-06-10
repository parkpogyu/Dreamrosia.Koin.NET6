using Dreamrosia.Koin.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "심볼정보")]
    public class Symbol
    {
        public string market { get; set; }

        public string korean_name { get; set; }

        public string english_name { get; set; }

        public MarketAlert market_warning { get; set; } = MarketAlert.None;

        public string unit_currency => GetUnitCurrency(market);

        public string code => GetCode(market);

        public static string GetUnitCurrency(string market)
        {
            if (string.IsNullOrEmpty(market)) { return string.Empty; }

            var splits = market.Split("-", StringSplitOptions.RemoveEmptyEntries);

            return splits.Count() == 2 ? splits[0] : string.Empty;
        }

        public static string GetCode(string market)
        {
            if (string.IsNullOrEmpty(market)) { return string.Empty; }

            var splits = market.Split("-", StringSplitOptions.RemoveEmptyEntries);

            return splits.Count() == 2 ? splits[1] : string.Empty;
        }
    }
}
