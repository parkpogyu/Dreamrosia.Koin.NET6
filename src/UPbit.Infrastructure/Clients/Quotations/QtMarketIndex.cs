using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "마켓인덱스")]
    public class QtMarketIndex : QuotaionClient<QtMarketIndex.QtParameter>
    {
        public QtMarketIndex()
        {
            URL = "https://crix-api-cdn.upbit.com/v1/crix/candles/days";
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?{1}", URL, QueryString(Parameter));
        }

        public async Task<IResult<IEnumerable<MarketIndex>>> GetMarketIndicesAsync()
        {
            return await GetMarketIndicesAsync(new QtParameter());
        }

        public async Task<IResult<IEnumerable<MarketIndex>>> GetMarketIndicesAsync(object parameter)
        {
            return await base.GetAsync<IEnumerable<MarketIndex>>(parameter);
        }

        public class QtParameter : IWebApiParameter
        {
            public string code { get; set; } = "IDX.UPBIT.UBMI";
            public int count { get; set; } = 10000;
        }
    }
}
