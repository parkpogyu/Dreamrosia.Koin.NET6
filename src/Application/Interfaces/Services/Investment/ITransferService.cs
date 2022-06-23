using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ITransferService
    {
        Task<IResult<IEnumerable<TransferDto>>> GetTransfersAsync(TransfersRequestDto model);
        Task<IResult<TransferDto>> GetLastTransferAsync(string userId, TransferType type);
        Task<IResult<int>> SaveTransfersAsync(string userId, IEnumerable<TransferDto> models);
    }
}