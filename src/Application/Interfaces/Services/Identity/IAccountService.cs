using Dreamrosia.Koin.Application.Interfaces.Common;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services.Identity
{
    public interface IAccountService : IService
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);
        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<IResult> RegisterAsync(RegisterRequest request, string origin);
        Task<IResult> RegisterOrUpdateKakaoUserAsync();
        Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request);
        Task<IResult<UserRolesResponse>> GetRolesAsync(string userId);
        Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);
        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);
    }
}