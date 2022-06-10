using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "지갑생성")]
    public class ExWalletPost : ExchangeClient<ExWalletPost.ExParameter>
    {
        public ResponseAsync Response { get; private set; }

        public ExWalletPost()
        {
            Parameter = new ExParameter();

            URL = string.Format("{0}/deposits/generate_coin_address", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<Wallet>> WalletPostAsync(object parameter)
        {
            return await base.PostAsync<Wallet>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            [Display(Name = "화폐코드")]
            public string currency { get; set; }
        }

        public class ResponseAsync : Wallet
        {
            [Display(Name = "성공여부")]
            public bool success { get; set; }

            [Display(Name = "내용")]
            public string message { get; set; }
        }

    }
}
