namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class TradingTermsEndpoints
    {
        public static string GetByUserId(string userId)
        {
            return $"api/tradingterms/{userId}";
        }

        public static string UpdateTradingTerms(string userId)
        {
            return $"api/tradingterms/update/{userId}";
        }
    }
}