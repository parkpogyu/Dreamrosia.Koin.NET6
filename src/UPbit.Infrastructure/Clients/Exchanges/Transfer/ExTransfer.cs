using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "입출금정보 조회")]
    public class ExTransfer : ExchangeClient<ExTransfer.ExParameter>
    {

        public ExTransfer()
        {
        }

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}/{1}?{2}", ApiUrl, Parameter.type, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public async Task<IResult<Transfer>> GetTransferAsync(object parameter)
        {
            return await base.GetAsync<Transfer>(parameter);
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 입출금 종류
            /// </summary>
            [JsonIgnore]
            public TransferType type { get; set; }

            /// <summary>
            /// 고유번호
            /// </summary>
            public string uuid { get; set; }

            /// <summary>
            /// 거래번호
            /// </summary>
            public string txid { get; set; }

            /// <summary>
            /// 화폐코드
            /// </summary>
            public string currency { get; set; }
        }

    }
}
