namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = "api/identity/account/register";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string ForgotPassword = "api/identity/account/forgot-password";
        public static string ResetPassword = "api/identity/account/reset-password";
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