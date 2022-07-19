using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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

        public async Task<IResult<IEnumerable<MarketIndexDto>>> GetMarketIndicesAsync(DateTime? head, DateTime? rear)
        {
            var response = await _httpClient.GetAsync(Routes.MarketEndpoints.GetMarketIndices(Convert.ToDateTime(head), Convert.ToDateTime(rear)));

            return await response.ToResult<IEnumerable<MarketIndexDto>>();
        }

        public async Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.MarketEndpoints.GetSymbols);

            return await response.ToResult<IEnumerable<SymbolDto>>();
        }

        public async Task<IResult<IEnumerable<DelistingSymbolDto>>> GetDelistingSymbolsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.MarketEndpoints.GetDelistingSymbols);

            return await response.ToResult<IEnumerable<DelistingSymbolDto>>();
        }

        public async Task<IResult> RegistDelistingSymbolAsync(DelistingSymbolDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.MarketEndpoints.RegistDelistingdSymbol, model);

            return await response.ToResult<string>();
        }

        public async Task<IResult> DeleteDelistingSymbolsAsync(IEnumerable<DelistingSymbolDto> models)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.MarketEndpoints.DeleteDelistingSymbols, models);

            return await response.ToResult<string>();
        }
    }
}