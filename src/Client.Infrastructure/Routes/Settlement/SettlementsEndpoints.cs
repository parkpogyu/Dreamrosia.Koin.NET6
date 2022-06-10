namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class SettlementsEndpoints
    {
        public static string GetBankingTransactions = $"api/settlements/bankingtransactions";

        public static string ImportBankingTransactions = $"api/settlements/bankingtransactions/import";

        public static string GetPoints(string userId)
        {
            return $"api/settlements/points/{userId}";
        }
    }
}