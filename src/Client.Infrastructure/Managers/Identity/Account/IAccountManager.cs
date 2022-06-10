using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Account
{
    public interface IAccountManager : IManager
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);

        Task<IResult<UserProfileDto>> GetProfileAsync(string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}