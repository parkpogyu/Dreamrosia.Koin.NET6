using Blazored.LocalStorage;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Client.Infrastructure.Authentication;
using Dreamrosia.Koin.Client.Infrastructure.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Routes;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Storage;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Authentication
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public AuthenticationManager(HttpClient httpClient,
                                     ILocalStorageService localStorage,
                                     AuthenticationStateProvider authenticationStateProvider,
                                     IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _localizer = localizer;
        }

        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();

            return state.User;
        }

        public async Task<IResult> Login(TokenRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.Get, model);

            var result = await response.ToResult<TokenResponse>();

            if (result.Succeeded)
            {
                await TokenToStorage(result.Data);

                await _authenticationStateProvider.GetAuthenticationStateAsync();

                return await Result.SuccessAsync();
            }
            else
            {
                return await Result.FailAsync(result.Messages);
            }
        }

        public async Task<IResult> KakaoLogin()
        {
            var response = await _httpClient.GetAsync(TokenEndpoints.GetKakao);

            var result = await response.ToResult<TokenResponse>();

            if (result.Succeeded)
            {
                await TokenToStorage(result.Data);

                return await Result.SuccessAsync();
            }
            else
            {
                return await Result.FailAsync(result.Messages);
            }
        }

        private async Task TokenToStorage(TokenResponse response)
        {
            await _localStorage.SetItemAsync(StorageConstants.Local.Token, response.Token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, response.RefreshToken);

            ((BlazorHeroStateProvider)this._authenticationStateProvider).MarkUserAsAuthenticated(response.Email);
        }

        public async Task<IResult> Logout()
        {
            await _localStorage.RemoveItemAsync(StorageConstants.Local.Token);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);

            ((BlazorHeroStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

            _httpClient.DefaultRequestHeaders.Authorization = null;

            return await Result.SuccessAsync();
        }

        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.Token);
            var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

            var response = await _httpClient.PostAsJsonAsync(Routes.TokenEndpoints.Refresh, new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });

            var result = await response.ToResult<TokenResponse>();

            if (!result.Succeeded)
            {
                throw new ApplicationException(_localizer["Something went wrong during the refresh token action"]);
            }

            token = result.Data.Token;
            refreshToken = result.Data.RefreshToken;

            await _localStorage.SetItemAsync(StorageConstants.Local.Token, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return token;
        }

        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

            if (string.IsNullOrEmpty(availableToken)) return string.Empty;

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals(ApplicationClaimTypes.Expire))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;

            if (diff.TotalMinutes <= 1) { return await RefreshToken(); }

            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }

    }
}