using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class SettlementManager : ISettlementManager
    {
        private readonly HttpClient _httpClient;

        public SettlementManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<BankingTransactionDto>>> GetBankingTransactionsAsync(BankingTransactionsRequestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.SettlementsEndpoints.GetBankingTransactions, model);

            return await response.ToResult<IEnumerable<BankingTransactionDto>>();
        }

        public async Task<IResult<int>> ImportBankingTransactionsAsync(UploadRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.SettlementsEndpoints.ImportBankingTransactions, model);

            return await response.ToResult<int>();
        }

        public async Task<IResult<IEnumerable<PointDto>>> GetPointsAsync(PointsRequestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.SettlementsEndpoints.GetPoints(model.UserId), model);

            return await response.ToResult<IEnumerable<PointDto>>();
        }
    }
}