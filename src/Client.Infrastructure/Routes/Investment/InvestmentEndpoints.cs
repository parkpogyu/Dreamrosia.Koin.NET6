namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class InvestmentEndpoints
    {
        public static string GetAssets(string userId)
        {
            return $"api/investments/assets/{userId}";
        }

        public static string GetOrders(string userId)
        {
            return $"api/investments/orders/{userId}";
        }

        public static string GetPositions(string userId)
        {
            return $"api/investments/positions/{userId}";
        }

        public static string GetTransfers(string userId)
        {
            return $"api/investments/transfers/{userId}";
        }
    }
}