using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class TestManager : ITestManager
    {
        private readonly HttpClient _httpClient;

        public TestManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<CandleDto>>> GetUPbitCandlesAsync()
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitCandles());

            return await response.ToResult<IEnumerable<CandleDto>>();
        }

        public async Task<IResult<IEnumerable<CrixDto>>> GetUPbitCrixesAsync()
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitCrixes());

            return await response.ToResult<IEnumerable<CrixDto>>();
        }

        public async Task<IResult<IEnumerable<SymbolDto>>> GetUPbitSymbolsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitSymbols());

            return await response.ToResult<IEnumerable<SymbolDto>>();
        }

        public async Task<IResult<IEnumerable<OrderDto>>> GetUPbitOrdersAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitOrders(userId));

            return await response.ToResult<IEnumerable<OrderDto>>();
        }

        public async Task<IResult<IEnumerable<PositionDto>>> GetUPbitPositionsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitPositions(userId));

            return await response.ToResult<IEnumerable<PositionDto>>();
        }

        public async Task<IResult<IEnumerable<TransferDto>>> GetUPbitTransfersAsync(string userId, TransferType type)
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetUPbitTransfers(userId, type));

            return await response.ToResult<IEnumerable<TransferDto>>();
        }

        public async Task<IResult<IEnumerable<SeasonSignalDto>>> GetSeasonSignalsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.GetSeasonSignals(userId));

            return await response.ToResult<IEnumerable<SeasonSignalDto>>();
        }

        public async Task<IResult<IEnumerable<SeasonSignalDto>>> UpdateSeasonSignalsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.TestsEndpoints.UpdateSeasonSignals(userId));

            return await response.ToResult<IEnumerable<SeasonSignalDto>>();
        }
    }
}