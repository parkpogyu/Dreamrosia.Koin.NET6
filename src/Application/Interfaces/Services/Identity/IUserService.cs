using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Identity
{
    public interface IUserService : IService
    {
        Task<IResult<UserDto>> GetAsync(string userId);
        Task<IResult<UserBriefDto>> GetUserBriefAsync(string userId);
        Task<IResult<UserFullInfoDto>> GetFullInfoAsync(string userId);
        Task<IResult<IEnumerable<UserFullInfoDto>>> GetFullInfosAsync(DateTime head, DateTime rear);
        Task<IResult<IEnumerable<FollowerDto>>> GetFollowersAsync(string userId, DateTime head, DateTime rear);
        Task<IResult<IEnumerable<BoasterDto>>> GetBoastersAsync(DateTime head, DateTime rear);
        Task<IResult<UserDto>> GetRecommenderAsync(RecommenderDto model);
        Task<IResult> UpdateRecommenderAsync(RecommenderDto model);

        Task<IResult<UserDto>> GetAccountHolderAsync(string userCode);
        Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model);

        Task<int> GetCountAsync();

        Task<IResult<UserDto>> GetProfileAsync(string userId);
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);
        Task<IResult<string>> GetProfilePictureAsync(string userId);
        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
        Task<string> ExportToExcelAsync(string searchString = "");
    }
}