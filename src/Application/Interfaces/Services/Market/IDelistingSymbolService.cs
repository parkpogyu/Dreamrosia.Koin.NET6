using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IDelistingSymbolService
    {
        Task<IResult<IEnumerable<DelistingSymbolDto>>> GetDelistingSymbolsAsync();
        Task<IResult> RegistDelistingSymbolAsync(DelistingSymbolDto model);
        Task<IResult> DeleteDelistingSymbolsAsync(IEnumerable<DelistingSymbolDto> models);
        Task<IResult> CollectDelistingSymbolsAsync();
    }
}