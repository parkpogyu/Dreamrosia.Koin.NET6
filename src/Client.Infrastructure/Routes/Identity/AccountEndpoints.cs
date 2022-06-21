namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class AccountEndpoints
    {
        public static string Register = "api/identity/account/register";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string ForgotPassword = "api/identity/account/forgot-password";
        public static string ResetPassword = "api/identity/account/reset-password";
        public static string ToggleUserStatus = "api/identity/account/toggle-status";

        public static string GetUserRoles(string userId)
        {
            return $"api/identity/account/roles/{userId}";
        }
    }
}