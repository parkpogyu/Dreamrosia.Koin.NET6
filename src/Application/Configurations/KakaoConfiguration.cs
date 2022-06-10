namespace Dreamrosia.Koin.Application.Configurations
{
    public class KakaoConfiguration
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public static string TockenName => "access_token";

        public static string LogoutUrl => "https://kapi.kakao.com/v1/user/logout";

        public static string SignoutUrl => "https://kauth.kakao.com/oauth/logout";
    }
}