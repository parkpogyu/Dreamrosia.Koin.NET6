using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class TestsEndpoints
    {
        public static string GetUPbitOrders(string userId)
        {
            return $"api/tests/orders/{userId}";
        }

        public static string GetUPbitPositions(string userId)
        {
            return $"api/tests/positions/{userId}";
        }

        public static string GetUPbitTransfers(string userId, TransferType type)
        {
            return $"api/tests/{type.ToString()}s/{userId}";
        }

        public static string GetUPbitCandles()
        {
            return $"api/tests/candles";
        }

        public static string GetUPbitCrixes()
        {
            return $"api/tests/crixes";
        }

        public static string GetUPbitSymbols()
        {
            return $"api/tests/symbols";
        }

        public static string GetSeasonSignals(string userId)
        {
            return $"api/tests/seasonsignals/{userId}";
        }

        public static string UpdateSeasonSignals(string userId)
        {
            return $"api/tests/seasonsignals/update/{userId}";
        }
    }
}