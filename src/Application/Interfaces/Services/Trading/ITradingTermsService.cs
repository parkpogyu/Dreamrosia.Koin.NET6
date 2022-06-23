using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ITradingTermsService
    {
        Task<IResult<TradingTermsDto>> GetTradingTermsAsync(string userId);
        Task<IResult> UpdateTradingTermsAsync(TradingTermsDto model);
    }
}