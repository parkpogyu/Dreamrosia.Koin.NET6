namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class UPbitKeysEndpoints
    {
        public static string GetByUserId(string userId)
        {
            return $"api/upbitkeys/{userId}";
        }

        public static string UpdateUPbitKey(string userId)
        {
            return $"api/upbitkeys/update/{userId}";
        }

        public static string GetAllowedIPs => $"api/upbitkeys/allowedips";

        public static string TestUPbitKey(string userId)
        {
            return $"api/upbitkeys/test/{userId}";
        }
    }
}