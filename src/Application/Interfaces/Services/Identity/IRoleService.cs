﻿using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleResponse>>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<Result<RoleResponse>> GetByIdAsync(string id);
        Task<Result<string>> SaveAsync(RoleRequest request);
        Task<Result<string>> DeleteAsync(string id);
        Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId);
        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}