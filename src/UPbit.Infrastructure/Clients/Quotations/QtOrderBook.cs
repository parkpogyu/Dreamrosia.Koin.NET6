using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{

    [Display(Name = "호가정보 조회")]
    public class QtOrderBook : QuotaionClient<QtOrderBook.QtParameter>
    {
        public QtOrderBook()
        {
            URL = string.Format("{0}/orderbook", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?markets={1}", URL, string.Join(",", Parameter.markets));
        }

        public async Task<IResult<IEnumerable<OrderBook>>> GetOrderBooksAsync(object parameter)
        {
            Parameter = (QtParameter)parameter;

            return await base.GetAsync<IEnumerable<OrderBook>>(parameter);
        }

        public class QtParameter : IWebApiParameter
        {
            /// <summary>
            /// 마켓코드 목록
            /// </summary>
            public List<string> markets { get; private set; } = new List<string>();
        }
    }
}
