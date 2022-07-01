using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "입출금 목록 조회")]
    public class ExTransfers : ExchangeClient<ExTransfers.ExParameter>
    {

        protected override void SetHeadersAndURI()
        {
            string query = QueryString(Parameter);

            URI = string.Format("{0}/{1}s?{2}", ApiUrl, Parameter.type, query);

            SetAuthenticationHeader(AuthorizationToken(query));
        }

        public Task<IResult<IEnumerable<Transfer>>> GetTransfersAsync(TransferType type, DateTime? to = null)
        {
            return type == TransferType.deposit ? GetDepositTransfersAsync(to) : GetWithdrawTransfersAsync(to);
        }

        public Task<IResult<IEnumerable<Transfer>>> GetDepositTransfersAsync(DateTime? to = null)
        {
            return GetAsync(TransferType.deposit, to);
        }

        public Task<IResult<IEnumerable<Transfer>>> GetWithdrawTransfersAsync(DateTime? to = null)
        {

            return GetAsync(TransferType.withdraw, to);
        }

        private async Task<IResult<IEnumerable<Transfer>>> GetAsync(TransferType type, DateTime? to)
        {
            Parameter = new ExParameter();

            Parameter.page = 1;
            Parameter.type = type;

            var transfers = new List<Transfer>();

            var limit = Convert.ToDateTime(to);

            IResult<IEnumerable<Transfer>> response = null;

            while (true)
            {
                response = await base.GetAsync<IEnumerable<Transfer>>(Parameter);

                if (!response.Succeeded) { break; }
                if (!response.Data.Any()) { break; }

                if (response.Data.Any(f => f.created_at <= limit))
                {
                    transfers.AddRange(response.Data.Where(f => f.created_at >= limit));

                    break;
                }
                else
                {
                    transfers.AddRange(response.Data);
                }

                if (response.Data.Count() <= MaxResponse) { Parameter.page++; }
            }

            if (response.Succeeded)
            {
                var items = transfers.Where(f => f.state == TransferState.done ||
                                                 f.state == TransferState.accepted);

                return await Result<IEnumerable<Transfer>>.SuccessAsync(items);
            }
            else
            {
                return response;
            }
        }

        public class ExParameter : IWebApiParameter
        {
            /// <summary>
            /// 거래종류
            /// </summary>
            [JsonIgnore]
            public TransferType type { get; set; }

            /// <summary>
            /// 화폐코드
            /// </summary>
            public string currency { get; set; }

            /// <summary>
            /// 거래상태
            /// </summary>
            public TransferState? state { get; set; }

            /// <summary>
            /// 고유번호 목록
            /// </summary>
            public List<string> uuids { get; set; }

            /// <summary>
            /// 거래번호 목록
            /// </summary>
            public List<string> txids { get; set; }

            /// <summary>
            /// 요청개수
            /// </summary>
            public int? limit { get; set; } = 100;

            /// <summary>
            /// 페이지
            /// </summary>
            public int? page { get; set; } = 1;

            /// <summary>
            /// 정렬방식
            /// </summary>
            public OrderBy? order_by { get; set; } = OrderBy.desc;
        }
    }
}
