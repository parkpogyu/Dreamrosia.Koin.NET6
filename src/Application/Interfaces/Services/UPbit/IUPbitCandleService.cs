using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUPbitCandleService
    {
        Task<IResult> GetCandlesAsync();
        Task<IResult> GetCandlesAsync(TimeFrames frame, IEnumerable<string> markets);
    }
}
