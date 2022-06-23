using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IUPbitKeyService
    {
        Task<IResult<UPbitKeyDto>> GetUPbitKeyAsync(string userId);
        Task<IResult<IEnumerable<UPbitKeyDto>>> GetUPbitKeysAsync(DateTime head, DateTime rear);
        Task<IResult> UpdateUPbitKeyAsync(UPbitKeyDto model);
        Task<IResult> OccurredFatalErrorAsync(string userId, string error);
        Task<IResult<UPbitKeyTestDto>> TestUPbitKeyAsync(UPbitKeyTestDto model);
    }
}