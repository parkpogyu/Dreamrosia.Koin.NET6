using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "지갑정보 조회")]
    public class ExWallet : ExchangeClient<ExWallet.ExParameter>
    {
        public ExWallet()
        {
            URL = string.Format("{0}/deposits/coin_address", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<Wallet>> GetWalletAsync(object parameter)
        {
            return await base.GetAsync<Wallet>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 화폐코드
            /// </summary>
            public string currency { get; set; }
        }

    }
}
