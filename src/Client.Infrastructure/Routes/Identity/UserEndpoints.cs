using System;

namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class UserEndpoints
    {
        public static string Get(string userId)
        {
            return $"api/identity/user/{userId}";
        }

        public static string GetSummaries(DateTime? head, DateTime? rear)
        {
            return $"api/identity/user?head={head?.ToString("d")}&rear={rear?.ToString("d")}";
        }

        public static string GetFollowers(string userId, DateTime? head, DateTime? rear)
        {
            return $"api/identity/user/followers/?id={userId}&head={head?.ToString("d")}&rear={rear?.ToString("d")}";
        }

        public static string GetBoasters(DateTime? head, DateTime? rear)
        {
            return $"api/identity/user/boasters/?head={head?.ToString("d")}&rear={rear?.ToString("d")}";
        }

        public static string GetRecommender(string userId)
        {
            return $"api/identity/user/recommender/{userId}";
        }

        public static string GetAccountHolder(string userId)
        {
            return $"api/identity/user/accountholder/{userId}";
        }

        public static string UpdateRecommender(string userId)
        {
            return $"api/identity/user/recommender/update/{userId}";
        }

        public static string GetSubscription(string userId)
        {
            return $"api/identity/user/subscription/{userId}";
        }

        public static string ChangeMembership(string userId)
        {
            return $"api/identity/user/membership/{userId}";
        }

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = "api/identity/user/export";
        public static string UpdateProfile = "api/identity/account/updateprofile";

        public static string GetProfile(string userId)
        {
            return $"api/identity/account/profile/{userId}";
        }

        public static string GetProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }

        public static string UpdateProfilePicture(string userId)
        {
            return $"api/identity/account/profile-picture/{userId}";
        }
    }
}