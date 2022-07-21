using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IPointService
    {
        Task<IResult<IEnumerable<PointDto>>> GetPointsAsync(PointsRequestDto model);
        Task<IResult> DailyDeductPoint();
    }
}