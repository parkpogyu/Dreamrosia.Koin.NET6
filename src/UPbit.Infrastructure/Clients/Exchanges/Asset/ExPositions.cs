using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "잔고 조회")]
    public class ExPositions : ExchangeClient<WebApiParameter>
    {
        public ExPositions()
        {
            URL = string.Format("{0}/accounts", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = URL;

            SetAuthenticationHeader(AuthorizationToken());
        }

        public async Task<IResult<IEnumerable<Position>>> GetPositionsAsync()
        {
            return await base.GetAsync<IEnumerable<Position>>();
        }
    }
}
