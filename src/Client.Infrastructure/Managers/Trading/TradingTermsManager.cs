using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class TradingTermsManager : ITradingTermsManager
    {
        private readonly HttpClient _httpClient;

        public TradingTermsManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<TradingTermsDto>> GetTradingTermsAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.TradingTermsEndpoints.GetByUserId(userId));

            return await response.ToResult<TradingTermsDto>();
        }

        public async Task<IResult> UpdateTradingTermsAsync(TradingTermsDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.TradingTermsEndpoints.UpdateTradingTerms(model.UserId), model);

            return await response.ToResult<string>();
        }
    }
}