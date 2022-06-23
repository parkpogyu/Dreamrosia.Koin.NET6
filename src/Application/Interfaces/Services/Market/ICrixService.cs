using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ICrixService
    {
        Task<IResult<IEnumerable<CrixDto>>> GetCrixesAsync();
        Task<IResult> SaveCrixesAsync(IEnumerable<CrixDto> models);
    }
}