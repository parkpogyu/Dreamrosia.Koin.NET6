using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface IMiningBotManager : IManager
    {
        Task<IResult<IEnumerable<MiningBotDto>>> GetMiningBotsAsync();
        Task<IResult<IEnumerable<MiningBotDto>>> GetTestMiningBotsAsync();
        Task<IResult<MiningBotDto>> GetMiningBotAsync(string userId);
    }
}