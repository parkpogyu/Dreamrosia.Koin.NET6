using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public class MiningBotManager : IMiningBotManager
    {
        private readonly HttpClient _httpClient;

        public MiningBotManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<MiningBotDto>>> GetMiningBotsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Routes.MiningBotsEndpoints.GetAll());

                return await response.ToResult<IEnumerable<MiningBotDto>>();

            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<IResult<IEnumerable<MiningBotDto>>> GetTestMiningBotsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(Routes.MiningBotsEndpoints.GetTestAll());

                return await response.ToResult<IEnumerable<MiningBotDto>>();

            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public async Task<IResult<MiningBotDto>> GetMiningBotAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.MiningBotsEndpoints.GetByUserId(userId));

            return await response.ToResult<MiningBotDto>();
        }

    }
}