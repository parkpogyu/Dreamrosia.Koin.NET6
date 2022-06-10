namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class MiningBotsEndpoints
    {
        public static string GetAll()
        {
            return $"api/terminal/miningbots";
        }

        public static string GetTestAll()
        {
            return $"api/terminal/miningbots/test";
        }

        public static string GetByUserId(string userId)
        {
            return $"api/terminal/miningbots/{userId}";
        }
    }
}