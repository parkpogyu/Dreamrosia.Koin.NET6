using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "지갑 목록 조회")]
    public class ExWallets : ExchangeClient<WebApiParameter>
    {
        public ExWallets()
        {
            URL = string.Format("{0}/deposits/coin_addresses", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = URL;

            SetAuthenticationHeader(AuthorizationToken());
        }

        public async Task<IResult<IEnumerable<Wallet>>> GetWalletsAsync()
        {
            return await base.GetAsync<IEnumerable<Wallet>>();
        }
    }
}
