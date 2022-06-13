using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IMiningBotService
    {
        Task<IResult<IEnumerable<MiningBotDto>>> GetMiningBotsAsync();

        Task<IResult<TradingTermsExtensionDto>> GetTradingTermsAsync(MiningBotDto model);

        Task<IResult<TradingTermsExtensionDto>> GetTestTradingTermsAsync(MiningBotDto model, string userId);

        Task MappingMiningBotAsync();
    }
}