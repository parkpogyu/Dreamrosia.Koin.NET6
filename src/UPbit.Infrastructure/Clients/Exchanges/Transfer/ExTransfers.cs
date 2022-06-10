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
            [JsonIgnore]
            public TransferType type { get; set; }

            [Display(Name = "화폐코드")]
            public string currency { get; set; }

            [Display(Name = "상태")]
            public TransferState? state { get; set; }

            [Display(Name = "고유번호목록")]
            public List<string> uuids { get; set; }

            [Display(Name = "거래번호목록")]
            public List<string> txids { get; set; }

            [Display(Name = "요청개수")]
            public int? limit { get; set; } = 100;

            [Display(Name = "페이지")]
            public int? page { get; set; } = 1;

            [Display(Name = "정령방식")]
            public OrderBy? order_by { get; set; } = OrderBy.desc;
        }
    }
}
