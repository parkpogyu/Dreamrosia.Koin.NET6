using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class UnitTestKakao
    {
        public UnitTestKakao()
        {
        }


        [TestMethod]
        public async Task TestKakaoLogin()
        {
            var jstHandler = new JwtSecurityTokenHandler();

            var jst = jstHandler.ReadJwtToken("Ekz4awYiWTL8VQM3GCzR9xP6JUZ1Gzesque7YQiVCilxIQAAAYFLXPfJ");
        }
    }

    public class KakaoApiEndPoint
    {
        // API Ű
        public const string KakaoRestApiKey = "6f3f934301e8f7d34edc0183a10dfba5";
        public const string KakaoSendMessageKey = "�������� ���ø� Ű";

        // �����̷�Ʈ url
        public const string KakaoRedirectUrl = "http://dreamrosia.co.kr";

        // �α��� url
        public const string KakaoLogInUrl = "https://kauth.kakao.com/oauth/authorize?client_id=" + KakaoRestApiKey + "&redirect_uri=" + KakaoRedirectUrl + "&response_type=code";

        // ��Ʈ url
        public const string KakaoHostOAuthUrl = "https://kauth.kakao.com";
        public const string KakaoHostApiUrl = "https://kapi.kakao.com";

        // �̺�Ʈ url
        public const string KakaoOAuthUrl = "/oauth/token"; // AccessToken
        public const string KakaoUnlinkUrl = "/v1/user/unlink"; // LogOut
        public const string KakaoTemplateMessageUrl = "/v2/api/talk/memo/send"; // Template �޽���
        public const string KakaoDefaultMessageUrl = "/v2/api/talk/memo/default/send"; // Default �޽���
        public const string KakaoUserDataUrl = "/v2/user/me"; // ����� ������
    }
}
