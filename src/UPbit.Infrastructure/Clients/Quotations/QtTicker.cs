using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "현재가 조회")]
    public class QtTicker : QuotaionClient<QtTicker.QtParameter>
    {
        public QtTicker()
        {
            URL = string.Format("{0}/ticker", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?markets={1}", URL, string.Join(",", Parameter.markets));
        }

        public async Task<IResult<IEnumerable<Ticker>>> GetTickersAsync(object parameter)
        {
            Parameter = (QtParameter)parameter;

            return await base.GetAsync<IEnumerable<Ticker>>(parameter);
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
