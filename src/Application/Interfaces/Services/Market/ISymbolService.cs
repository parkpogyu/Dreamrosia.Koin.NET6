using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ISymbolService
    {
        Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync();

        Task<IResult<IEnumerable<string>>> GetSymbolCodesAsync();

        Task<IResult> SaveSymbolsAsync(IEnumerable<SymbolDto> models);
    }
}