using System;

namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class UPbitKeysEndpoints
    {
        public static string GetUPbitKey(string userId)
        {
            return $"api/upbitkeys/{userId}";
        }

        public static string GetUPbitKeys(DateTime head, DateTime rear)
        {
            return $"api/upbitkeys?head={head:d}&rear={rear:d}";
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