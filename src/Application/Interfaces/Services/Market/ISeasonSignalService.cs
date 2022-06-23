using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ISeasonSignalService
    {
        Task<IResult<IEnumerable<SeasonSignalDto>>> GetSeasonSignalsAsync(string userId);
        Task<IResult<int>> UpdateSeasonSignalsAsync(string userId);
    }
}