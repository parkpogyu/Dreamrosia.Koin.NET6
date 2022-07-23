using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUnlistedSymbolService
    {
        Task<IResult> FilterUnlistedSymbolsAsync();
        Task<IResult> GetSymbolsIdAsync();
        Task<IResult> GetPriceAsync();
    }
}