using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "인증키 목록 조회")]
    public class ExAccessKeys : ExchangeClient<WebApiParameter>
    {
        public ExAccessKeys()
        {
            URL = string.Format("{0}/api_keys", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = URL;

            SetAuthenticationHeader(AuthorizationToken());
        }

        public async Task<IResult<IEnumerable<UPbitKey>>> GetAccessKeysAsync()
        {
            return await base.GetAsync<IEnumerable<UPbitKey>>();
        }
    }
}
