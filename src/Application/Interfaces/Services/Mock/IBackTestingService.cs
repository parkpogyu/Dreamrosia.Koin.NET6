using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IBackTestingService
    {
        Task<IResult<byte[]>> GetBackTestingAsync(BackTestingRequestDto model);
    }
}