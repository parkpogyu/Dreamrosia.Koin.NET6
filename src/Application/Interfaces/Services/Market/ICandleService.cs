using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface ICandleService
    {
        Task<IResult<IEnumerable<CandleDto>>> GetCandlesAsync(string market, DateTime head, DateTime rear);
        Task<IResult<IEnumerable<LastCandleDto>>> GetLastCandlesAsync(IEnumerable<string> models);
        Task<IResult<IEnumerable<CandleDto>>> GetTodayCandlesAsync(bool exist = true);
        Task<IResult> SaveCandlesAsync(IEnumerable<CandleDto> models);
    }
}