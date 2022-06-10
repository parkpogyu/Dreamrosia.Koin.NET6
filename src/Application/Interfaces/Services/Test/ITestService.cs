using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ITestService
    {
        Task<IResult> GetUPbitSymbolsAsync();

        Task<IResult> GetUPbitCrixesAsync();

        Task<IResult> GetUPbitCandlesAsync();

        Task<IResult> GetUPbitPositionsAsync(string userId);

        Task<IResult> GetUPbitOrdersAsync(string userId);

        Task<IResult> GetUPbitTransfersAsync(string userId, TransferType type);

        Task<IResult> GetSeasonSignalsAsync(string userId);

        Task<IResult> UpdateSeasonSignalsAsync(string userId);
    }
}