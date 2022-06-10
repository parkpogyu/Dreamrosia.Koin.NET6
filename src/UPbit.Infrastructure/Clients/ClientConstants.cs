using System.Collections.Generic;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    public static class ClientConstants
    {
        public static int MaxCount => 200;

        public static readonly Dictionary<string, string> FatalErrors = new Dictionary<string, string>()
        {
            { "invalid_access_key", "잘못된 엑세스 키입니다." },
            { "no_authorization_i_p", "허용되지 않은 IP 주소입니다." },
            { "out_of_scope", "허용되지 않은 기능입니다." },
            { "expired_access_key", "API 키가 만료되었습니다." },
        };
    }
}
