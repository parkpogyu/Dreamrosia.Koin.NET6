using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class InvestmentManager : IInvestmentManager
    {
        private readonly HttpClient _httpClient;

        public InvestmentManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<AssetReportDto>> GetAssetsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.InvestmentEndpoints.GetAssets(userId));

            return await response.ToResult<AssetReportDto>();
        }

        public async Task<IResult<IEnumerable<OrderDto>>> GetOrdersAsync(OrdersRequestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.InvestmentEndpoints.GetOrders(model.UserId), model);

            return await response.ToResult<IEnumerable<OrderDto>>();
        }

        public async Task<IResult<PositionsDto>> GetPositionsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.InvestmentEndpoints.GetPositions(userId));

            return await response.ToResult<PositionsDto>();
        }

        public async Task<IResult<IEnumerable<TransferDto>>> GetTransfersAsync(TransfersRequestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.InvestmentEndpoints.GetTransfers(model.UserId), model);

            return await response.ToResult<IEnumerable<TransferDto>>();
        }
    }
}