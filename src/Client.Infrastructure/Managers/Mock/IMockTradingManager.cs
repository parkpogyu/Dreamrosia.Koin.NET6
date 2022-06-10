using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface IMockTradingManager : IManager
    {
        Task<IResult<byte[]>> GetBackTestingAsync(BackTestRequestDto model);
    }
}