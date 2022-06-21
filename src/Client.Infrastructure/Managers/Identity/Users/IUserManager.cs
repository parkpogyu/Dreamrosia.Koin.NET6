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

        Task<IResult<SubscriptionDto>> GetSubscriptionAsync(string userId);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariesAsync(DateTime? head = null, DateTime? rear = null);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetFollowersAsync(string userId, DateTime? head = null, DateTime? rear = null);

        Task<IResult<IEnumerable<UserSummaryDto>>> GetBoastersAsync(DateTime? head = null, DateTime? rear = null);

        Task<IResult<UserDto>> GetRecommenderAsync(RecommenderDto model);

        Task<IResult> UpdateRecommenderAsync(RecommenderDto model);

        Task<IResult<UserDto>> GetAccountHolderAsync(string userCode);

        Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);

        Task<IResult<UserDto>> GetProfileAsync(string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);

        Task<string> ExportToExcelAsync(string searchString = "");
    }
}