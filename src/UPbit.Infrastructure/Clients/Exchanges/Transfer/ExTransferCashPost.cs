using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "입출금")]
    public class ExTransferCashPost : ExchangeClient<ExTransferCashPost.ExParameter>
    {
        public ExTransferCashPost()
        {
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}/{1}s/krw?{2}", ApiUrl, Parameter.type, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        //public async Task PostAsync(TransferType type)
        //{
        //    try
        //    {

        //        Response = await SendAsync<Transfer>(HttpMethodPost);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log.Error(ex, ex.Message);
        //    }
        //}

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 거래종류
            /// </summary>
            public TransferType type { get; set; }

            /// <summary>
            /// 거래금액/수량
            /// </summary>
            public int amount { get; set; } = 5000;
        }

    }
}
