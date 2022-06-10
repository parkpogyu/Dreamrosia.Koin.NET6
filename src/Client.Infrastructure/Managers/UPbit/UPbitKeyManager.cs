using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class UPbitKeyManager : IUPbitKeyManager
    {
        private readonly HttpClient _httpClient;

        public UPbitKeyManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<UPbitKeyDto>> GetUPbitKeyAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UPbitKeysEndpoints.GetByUserId(userId));

            return await response.ToResult<UPbitKeyDto>();
        }

        public async Task<IResult> UpdateUPbitKeyAsync(UPbitKeyDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UPbitKeysEndpoints.UpdateUPbitKey(model.UserId), model);

            return await response.ToResult<string>();
        }

        public async Task<IResult<IEnumerable<string>>> GetAllowedIPsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.UPbitKeysEndpoints.GetAllowedIPs);

            return await response.ToResult<IEnumerable<string>>();
        }

        public async Task<IResult<UPbitKeyTestDto>> TestUPbitKeyAsync(UPbitKeyTestDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UPbitKeysEndpoints.TestUPbitKey(model.UserId), model);

            return await response.ToResult<UPbitKeyTestDto>();
        }
    }
}