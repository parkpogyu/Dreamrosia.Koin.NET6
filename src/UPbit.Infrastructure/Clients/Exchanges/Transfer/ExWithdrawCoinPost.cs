using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "코인출금")]
    public class ExWithdrawCoinPost : ExchangeClient<ExWithdrawCoinPost.ExParameter>
    {
        public Transfer Response { get; set; }

        public ExWithdrawCoinPost()
        {
            URL = string.Format("{0}/withdraws/coin", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<Transfer>> WithdrawCoinPostAsync(object parameter)
        {
            return await base.PostAsync<Transfer>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            [Display(Name = "화폐코드")]
            public string currency { get; set; }

            [Display(Name = "출금수량")]
            public double amount { get; set; }

            [Display(Name = "지갑주소")]
            public string address { get; set; }

            [Display(Name = "보조주소")]
            public string secondary_address { get; set; }

            [JsonIgnore]
            [Display(Name = "출금유형")]
            public TransferTransaction transaction_type { get; set; } = TransferTransaction.@default;
        }

    }
}
