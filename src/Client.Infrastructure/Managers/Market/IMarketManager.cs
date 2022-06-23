using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface IMarketManager : IManager
    {
        Task<IResult<IEnumerable<CandleDto>>> GetCandlesAsync(string market, DateTime? head, DateTime? rear);

        Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync();
    }
}