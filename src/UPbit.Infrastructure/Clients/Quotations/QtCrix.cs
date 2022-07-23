using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "시가총액 목록 조회")]
    public class QtCrix : QuotaionClient<QtCrix.QtParameter>
    {
        public QtCrix()
        {
            URL = "https://crix-api-cdn.upbit.com/v1/crix/marketcap";
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?{1}", URL, QueryString(Parameter));
        }

        public async Task<IResult<IEnumerable<Crix>>> GetCrixesAsync()
        {
            return await GetCrixesAsync(new QtParameter());
        }

        public async Task<IResult<IEnumerable<Crix>>> GetCrixesAsync(object parameter)
        {
            return await base.GetAsync<IEnumerable<Crix>>(parameter);
        }

        public class QtParameter : IWebApiParameter
        {
            public string currency { get; set; } = Currency.KRW;
        }
    }
}
