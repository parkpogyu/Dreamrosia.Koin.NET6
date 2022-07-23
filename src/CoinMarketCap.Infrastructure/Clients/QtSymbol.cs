using Dreamrosia.Koin.CoinMarketCap.Infrastructure.Models;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients
{
    [Display(Name = "코인 목록 조회")]
    public class QtSymbol : CoinMarketCapWebApiClient<QtSymbol.QtParameter>
    {
        public QtSymbol()
        {
            URL = string.Format("{0}/listing", ApiUrl);
        }

        public async Task<IResult<IEnumerable<Symbol>>> GetSymbolsAsync()
        {
            var response = await base.GetAsync<Symbols>(new QtParameter());

            if (response.Succeeded)
            {
                response = await base.GetAsync<Symbols>(new QtParameter()
                {
                    limit = Convert.ToInt32(response.Data.List.totalCount),
                });

                if (response.Succeeded)
                {
                    return await Result<IEnumerable<Symbol>>.SuccessAsync(response.Data.List.Symbols);
                }
                else
                {
                    return await Result<IEnumerable<Symbol>>.FailAsync(response.Messages);
                }
            }
            else
            {
                return await Result<IEnumerable<Symbol>>.FailAsync(response.Messages);
            }
        }

        public class QtParameter : IWebApiParameter
        {
            public int limit { get; set; } = 1;
        }
    }
}
