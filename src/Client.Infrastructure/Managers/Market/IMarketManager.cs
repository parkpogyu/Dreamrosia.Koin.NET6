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
        Task<IResult<IEnumerable<MarketIndexDto>>> GetMarketIndicesAsync(DateTime? head, DateTime? rear);
        Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync();

        Task<IResult<IEnumerable<DelistingSymbolDto>>> GetDelistingSymbolsAsync();
        Task<IResult> RegistDelistingSymbolAsync(DelistingSymbolDto model);
        Task<IResult> DeleteDelistingSymbolsAsync(IEnumerable<DelistingSymbolDto> models);
    }
}