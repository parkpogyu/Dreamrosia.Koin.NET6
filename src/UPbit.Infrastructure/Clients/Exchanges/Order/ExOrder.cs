using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "주문정보 조회")]
    public class ExOrder : ExchangeClient<ExOrder.ExParameter>
    {
        public ExOrder()
        {
            URL = string.Format("{0}/order", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}?{1}", URL, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<OrderDetails>> GetOrderAsync(object parameter)
        {
            return await base.GetAsync<OrderDetails>(parameter);
        }

        // uuid 혹은 identifier 둘 중 하나의 값이 반드시 포함되어야 합니다.
        public class ExParameter : IWebApiParameter
        {
            [Display(Name = "주문번호")]
            public string uuid { get; set; }

            [Display(Name = "조회번호")]
            public string identifier { get; set; }
        }
    }
}
