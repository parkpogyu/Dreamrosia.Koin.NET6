namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class TokenEndpoints
    {
        public static string Get => "api/identity/token";
        public static string Refresh => "api/identity/token/refresh";
        public static string GetKakao => "api/identity/token/kakao";
        public static string KakaoSignin => "api/identity/token/kakao-signin";
        public static string KakaoSignout => "api/identity/token/kakao-signout";
    }
}