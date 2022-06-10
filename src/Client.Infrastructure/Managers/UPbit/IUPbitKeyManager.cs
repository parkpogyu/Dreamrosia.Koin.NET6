﻿using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface IUPbitKeyManager : IManager
    {
        Task<IResult<UPbitKeyDto>> GetUPbitKeyAsync(string userId);

        Task<IResult> UpdateUPbitKeyAsync(UPbitKeyDto model);

        Task<IResult<IEnumerable<string>>> GetAllowedIPsAsync();

        Task<IResult<UPbitKeyTestDto>> TestUPbitKeyAsync(UPbitKeyTestDto model);
    }
}