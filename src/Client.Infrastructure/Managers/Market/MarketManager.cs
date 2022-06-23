using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class MarketManager : IMarketManager
    {
        private readonly HttpClient _httpClient;

        public MarketManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<CandleDto>>> GetCandlesAsync(string market, DateTime? head, DateTime? rear)
        {
            var response = await _httpClient.GetAsync(Routes.MarketEndpoints.GetCandles(market, Convert.ToDateTime(head), Convert.ToDateTime(rear)));

            return await response.ToResult<IEnumerable<CandleDto>>();
        }

        public async Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.MarketEndpoints.GetSymbols);

            return await response.ToResult<IEnumerable<SymbolDto>>();
        }
    }
}