using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Users
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient _httpClient;

        public UserManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.Get(userId));

            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<UserDetailDto>> GetDetailAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetDetail(userId));

            return await response.ToResult<UserDetailDto>();
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariseAsync(DateTime? head = null, DateTime? rear = null)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetSummarise(head, rear));

            return await response.ToResult<IEnumerable<UserSummaryDto>>();
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetFollowersAsync(string userId, DateTime? head = null, DateTime? rear = null)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetFollowers(userId, head, rear));

            return await response.ToResult<IEnumerable<UserSummaryDto>>();
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetBoastersAsync(DateTime? head = null, DateTime? rear = null)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetBoasters(head, rear));

            return await response.ToResult<IEnumerable<UserSummaryDto>>();
        }

        public async Task<IResult<UserResponse>> GetRecommenderAsync(RecommenderDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.GetRecommender(model.UserId), model);

            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult> UpdateRecommenderAsync(RecommenderDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.UpdateRecommender(model.UserId), model);

            return await response.ToResult();
        }

        public async Task<IResult<UserResponse>> GetAccountHolderAsync(string userCode)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetAccountHolder(userCode));

            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.ChangeMembership(model.UserId), model);

            return await response.ToResult<MembershipDto>();
        }

        public async Task<IResult> RegisterUserAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.Register, request);

            return await response.ToResult();
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.UserEndpoints.ToggleUserStatus, request);

            return await response.ToResult();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            var response = await _httpClient.GetAsync(Routes.UserEndpoints.GetUserRoles(userId));

            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync(Routes.UserEndpoints.GetUserRoles(request.UserId), request);

            return await response.ToResult<UserRolesResponse>();
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString) ?
                           Routes.UserEndpoints.Export :
                           Routes.UserEndpoints.ExportFiltered(searchString));

            var data = await response.Content.ReadAsStringAsync();

            return data;
        }
    }
}