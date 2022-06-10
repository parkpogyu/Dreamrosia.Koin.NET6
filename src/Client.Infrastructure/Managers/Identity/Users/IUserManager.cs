using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Users
{
    public interface IUserManager : IManager
    {
        Task<IResult<UserResponse>> GetAsync(string userId);

        Task<IResult<UserDetailDto>> GetDetailAsync(string userId);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariseAsync(DateTime? head = null, DateTime? rear = null);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetFollowersAsync(string userId, DateTime? head = null, DateTime? rear = null);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetBoastersAsync(DateTime? head = null, DateTime? rear = null);

        Task<IResult<UserResponse>> GetRecommenderAsync(RecommenderDto model);

        Task<IResult> UpdateRecommenderAsync(RecommenderDto model);

        Task<IResult<UserResponse>> GetAccountHolderAsync(string userCode);

        Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model);

        Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);

        Task<IResult> RegisterUserAsync(RegisterRequest request);

        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);

        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);


        Task<string> ExportToExcelAsync(string searchString = "");
    }
}