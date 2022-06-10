using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult<UserProfileDto>> GetProfileAsync(string userId);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}