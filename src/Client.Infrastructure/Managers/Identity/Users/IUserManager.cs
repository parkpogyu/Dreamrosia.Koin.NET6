using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Users
{
    public interface IUserManager : IManager
    {
        Task<IResult<UserDto>> GetAsync(string userId);
        Task<IResult<UserBriefDto>> GetUserBriefAsync(string userId);
        Task<IResult<UserFullInfoDto>> GetFullInfoAsync(string userId);
        Task<IResult<IEnumerable<UserFullInfoDto>>> GetFullInfosAsync(DateTime? head, DateTime? rear);
        Task<IResult<IEnumerable<FollowerDto>>> GetFollowersAsync(string userId, DateTime? head, DateTime? rear);
        Task<IResult<IEnumerable<BoasterDto>>> GetBoastersAsync(DateTime? head, DateTime? rear);
        Task<IResult<UserDto>> GetRecommenderAsync(RecommenderDto model);
        Task<IResult> UpdateRecommenderAsync(RecommenderDto model);
        Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model);
        Task<IResult<UserDto>> GetAccountHolderAsync(string userCode);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);
        Task<IResult<UserDto>> GetProfileAsync(string userId);
        Task<IResult<string>> GetProfilePictureAsync(string userId);
        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
        Task<string> ExportToExcelAsync(string searchString = "");
    }
}