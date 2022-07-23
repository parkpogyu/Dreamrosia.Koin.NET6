using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "주문가능정보 조회")]
    public class ExOrderAvailable : ExchangeClient<ExOrderAvailable.ExParameter>
    {
        public ExOrderAvailable()
        {
            URL = string.Format("{0}/orders/chance", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<OrderAvailable>> GetOrderAvailableAsync(object parameter)
        {
            return await base.GetAsync<OrderAvailable>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 마켓코드
            /// </summary>
            public string market { get; set; }
        }

    }
}
