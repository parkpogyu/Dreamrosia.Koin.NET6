using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Routes;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Authentication
{
    public partial class Login
    {
        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();

            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                var user = state.User;

                var id = user?.GetUserId();

                if (string.IsNullOrEmpty(id))
                {
                    var result = await _authenticationManager.KakaoLogin();

                    if (result.Succeeded)
                    {
                        _navigationManager.NavigateTo("/", forceLoad: true);
                    }
                }
                else
                {
                    _navigationManager.NavigateTo("/");
                }
            }
        }

        private void Signin()
        {
            _navigationManager.NavigateTo(TokenEndpoints.KakaoSignin, forceLoad: true);
        }
    }
}