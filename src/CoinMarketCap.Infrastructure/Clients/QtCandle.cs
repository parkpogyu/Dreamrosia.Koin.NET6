using Dreamrosia.Koin.CoinMarketCap.Infrastructure.Models;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.WebApi.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients
{
    [Display(Name = "캔들 조회")]
    public class QtCandle : CoinMarketCapWebApiClient<QtCandle.QtParameter>
    {
        public QtCandle()
        {
            URL = string.Format("{0}/historical", ApiUrl);
        }

        public async Task<IResult<IEnumerable<Candle>>> GetCandlesAsync(object parameter)
        {

            var response = await base.GetAsync<Candles>(parameter);

            if (response.Succeeded)
            {
                return await Result<IEnumerable<Candle>>.SuccessAsync(response.Data.List.Quetes
                                                                              .Select(f => f.Candle).ToArray());
            }
            else
            {
                return await Result<IEnumerable<Candle>>.FailAsync(response.Messages);
            }
        }

        public class QtParameter : IWebApiParameter
        {
            public int id { get; set; }
            public long timeStart { get; set; }
            public long timeEnd { get; set; }
            public string convertId => "2798";
        }
    }
}
