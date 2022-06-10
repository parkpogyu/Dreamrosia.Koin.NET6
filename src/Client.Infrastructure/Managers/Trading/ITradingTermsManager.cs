using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface ITradingTermsManager : IManager
    {
        Task<IResult<TradingTermsDto>> GetTradingTermsAsync(string userId);

        Task<IResult> UpdateTradingTermsAsync(TradingTermsDto model);
    }
}