using Blazored.LocalStorage;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Role;
using Dreamrosia.Koin.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Authentication
{
    public class BlazorHeroStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public BlazorHeroStateProvider(HttpClient httpClient,
                                       ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public void MarkUserAsAuthenticated()
        {
            //var authenticatedUser = new ClaimsPrincipal(
            //    new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Email, email)
            //    }, "apiauth"));

            //var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());

            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            AuthenticationStateUser = anonymousUser;

            NotifyAuthenticationStateChanged(authState);
        }

        public ClaimsPrincipal GetAuthenticationStateUser() => AuthenticationStateUser ?? new ClaimsPrincipal(new ClaimsIdentity());

        private ClaimsPrincipal AuthenticationStateUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.Token);

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = GetClaimsFromJwt(token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            AuthenticationStateUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            return new AuthenticationState(AuthenticationStateUser);
        }

        public bool IsInRole(string role)
        {
            return AuthenticationStateUser.IsInRole(role);
        }

        public bool IsHasClaim(string type, string value)
        {
            return AuthenticationStateUser.HasClaim(type, value);
        }

        private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        {
            var jstHandler = new JwtSecurityTokenHandler();

            var jst = jstHandler.ReadJwtToken(jwt);

            return jst.Claims.Where(f => f.Type.Equals(ClaimTypes.NameIdentifier) ||
                                         f.Type.Equals(ClaimTypes.Name) ||
                                         f.Type.Equals(ClaimTypes.Email) ||
                                         f.Type.Equals(ClaimTypes.MobilePhone) ||
                                         f.Type.Equals(ClaimTypes.Role) ||
                                         f.Type.Equals(ApplicationClaimTypes.Permission) ||
                                         f.Type.Equals(ApplicationClaimTypes.ProfielImange) ||
                                         f.Type.Equals(ApplicationClaimTypes.Expire)).ToArray();
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}