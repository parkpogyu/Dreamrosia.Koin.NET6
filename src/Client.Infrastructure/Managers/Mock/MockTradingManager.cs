using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class BackTestingManager : IBackTestingManager
    {
        private readonly HttpClient _httpClient;

        public BackTestingManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<byte[]>> GetBackTestingAsync(BackTestingRequestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.BackTestingEndpoints.GetBackTesting, model);

            return await response.ToResult<byte[]>();
        }
    }
}