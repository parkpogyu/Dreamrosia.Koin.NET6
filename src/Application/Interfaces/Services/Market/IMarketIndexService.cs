using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IMarketIndexService
    {
        Task<IResult<IEnumerable<MarketIndexDto>>> GetMarketIndicesAsync(DateTime head, DateTime rear);
        Task<IResult> SaveMarketIndicesAsync(IEnumerable<MarketIndexDto> models);
    }
}