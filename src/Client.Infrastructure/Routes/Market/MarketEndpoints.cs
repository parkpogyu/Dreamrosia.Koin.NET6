using System;

namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class MarketEndpoints
    {
        public static string ExportSymbols = "api/market/symbols/export";

        public static string GetSymbols = "api/market/symbols";

        public static string GetCandles(string market, DateTime? head, DateTime? rear)
        {
            return $"api/market/candles?market={market}&head={head?.ToString("yyyy-MM-dd")}&rear={rear?.ToString("yyyy-MM-dd")}";
        }

        public static string ExportCandles = "api/market/candles/export";
    }
}