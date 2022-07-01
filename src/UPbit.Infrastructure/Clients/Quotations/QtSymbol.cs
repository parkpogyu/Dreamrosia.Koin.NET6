using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    [Display(Name = "코인 목록 조회")]
    public class QtSymbol : QuotaionClient<QtSymbol.QtParameter>
    {
        public QtSymbol()
        {
            URL = string.Format("{0}/market/all", ApiUrl);
        }

        protected override void SetHeadersAndURI()
        {
            URI = string.Format("{0}?{1}", URL, QueryString(Parameter));
        }

        public async Task<IResult<IEnumerable<Symbol>>> GetSymbolsAsync()
        {
            return await GetSymbolsAsync(new QtParameter());
        }

        public async Task<IResult<IEnumerable<Symbol>>> GetSymbolsAsync(object parameter)
        {
            Parameter = (QtParameter)parameter;

            IResult<IEnumerable<Symbol>> response = await base.GetAsync<IEnumerable<Symbol>>(parameter);

            var symbols = response.Data.Where(f => f.unit_currency.Equals(Parameter.unit_currency));

            if (response.Succeeded)
            {
                return await Result<IEnumerable<Symbol>>.SuccessAsync(symbols);
            }
            else
            {
                return response;
            }
        }

        public class QtParameter : IWebApiParameter
        {
            /// <summary>
            /// 기준화폐코드
            /// </summary>
            [JsonIgnore]
            public string unit_currency { get; set; } = Currency.KRW;

            /// <summary>
            /// 상세조회
            /// </summary>
            public bool isDetails { get; set; } = true;
        }
    }
}
