using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface ITestManager : IManager
    {
        Task<IResult<IEnumerable<CrixDto>>> GetUPbitCrixesAsync();

        Task<IResult<IEnumerable<SymbolDto>>> GetUPbitSymbolsAsync();

        Task<IResult<IEnumerable<CandleDto>>> GetUPbitCandlesAsync();

        Task<IResult<IEnumerable<OrderDto>>> GetUPbitOrdersAsync(string userId);

        Task<IResult<IEnumerable<PositionDto>>> GetUPbitPositionsAsync(string userId);

        Task<IResult<IEnumerable<TransferDto>>> GetUPbitTransfersAsync(string userId, TransferType type);

        Task<IResult<IEnumerable<SeasonSignalDto>>> GetSeasonSignalsAsync(string userId);

        Task<IResult<IEnumerable<SeasonSignalDto>>> UpdateSeasonSignalsAsync(string userId);
    }
}