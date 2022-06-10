using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Identity
{
    public interface IUserService : IService
    {
        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<IResult<UserDetailDto>> GetDetailAsync(string userId);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariseAsync(DateTime head, DateTime rear);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetFollowersAsync(string userId, DateTime head, DateTime rear);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetBoastersAsync(DateTime head, DateTime rear);

        Task<IResult<UserResponse>> GetRecommenderAsync(RecommenderDto model);

        Task<IResult> UpdateRecommenderAsync(RecommenderDto model);

        Task<IResult<UserResponse>> GetAccountHolderAsync(string userCode);

        Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model);

        Task<int> GetCountAsync();

        Task<IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult> RegisterOrUpdateKakaoUserAsync();

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string id);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}