using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUPbitTickerService
    {
        void RequestTickers(IEnumerable<string> codes);
        Task<IResult<IEnumerable<TickerDto>>> GetTradePricesAsync();
        Task<IResult<int>> InitializeAsync();
    }
}
