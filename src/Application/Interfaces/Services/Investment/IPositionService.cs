using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IPositionService
    {
        Task<IResult<PositionsDto>> GetPositionsAsync(string userId);

        Task<IResult<int>> SavePositionsAsync(string userId, IEnumerable<PositionDto> models);
    }
}
