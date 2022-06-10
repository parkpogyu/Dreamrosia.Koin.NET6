using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Account
{
    public class AccountManager : IAccountManager
    {
        private readonly HttpClient _httpClient;

        public AccountManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.AccountEndpoints.ChangePassword, model);
            return await response.ToResult();
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountEndpoints.ForgotPassword, model);

            return await response.ToResult();
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountEndpoints.ResetPassword, request);

            return await response.ToResult();
        }


        public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest model)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.AccountEndpoints.UpdateProfile, model);
            return await response.ToResult();
        }

        public async Task<IResult<UserProfileDto>> GetProfileAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.AccountEndpoints.GetProfile(userId));

            return await response.ToResult<UserProfileDto>();
        }

        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.AccountEndpoints.GetProfilePicture(userId));
            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AccountEndpoints.UpdateProfilePicture(userId), request);

            return await response.ToResult<string>();
        }
    }
}