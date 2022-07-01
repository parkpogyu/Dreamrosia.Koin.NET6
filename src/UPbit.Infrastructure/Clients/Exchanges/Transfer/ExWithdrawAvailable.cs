using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "출금가능정보 조회")]
    public class ExWithdrawAvailable : ExchangeClient<ExWithdrawAvailable.ExParameter>
    {

        public ExWithdrawAvailable()
        {
            URL = string.Format("{0}/withdraws/chance", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<WithdrawAvailable>> GetWithdrawAvailableAsync(object parameter)
        {
            return await base.GetAsync<WithdrawAvailable>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 화폐코드
            /// </summary>
            public string currency { get; set; } = Currency.KRW;
        }
    }
}
