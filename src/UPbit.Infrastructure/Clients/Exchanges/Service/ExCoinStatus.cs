using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "코인상태 목록 조회")]
    public class ExCoinStatus : ExchangeClient<WebApiParameter>
    {
        public ExCoinStatus()
        {
            URL = string.Format("{0}/status/wallet", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = URL;

            SetAuthenticationHeader(AuthorizationToken());
        }

        public async Task<IResult<IEnumerable<CoinStatus>>> GetCoinStatusesAsync()
        {
            return await base.GetAsync<IEnumerable<CoinStatus>>();
        }
    }
}
