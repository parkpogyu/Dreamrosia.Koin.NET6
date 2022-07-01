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
            /// <summary>
            /// 화폐코드
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 출금수량
            /// </summary>
            public decimal amount { get; set; }

            /// <summary>
            /// 지갑주소
            /// </summary>
            public string address { get; set; }

            /// <summary>
            /// 보조주소
            /// </summary>
            public string secondary_address { get; set; }

            /// <summary>
            /// 출금유형
            /// </summary>
            [JsonIgnore]
            public TransferTransaction transaction_type { get; set; } = TransferTransaction.@default;
        }
    }
}
