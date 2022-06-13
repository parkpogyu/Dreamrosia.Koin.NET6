using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Routes;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Authentication
{
    public partial class Login
    {
        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();

            if (state.User.Claims.Any())
            {
                _navigationManager.NavigateTo("/");
            }
            else
            {
                var id = state.User?.GetUserId();

                if (string.IsNullOrEmpty(id))
                {
                    var result = await _authenticationManager.KakaoLogin();

                    if (result.Succeeded)
                    {
                        _navigationManager.NavigateTo("/", forceLoad: true);
                        //_navigationManager.NavigateTo("/");
                    }
                }
            }
        }

        private void Signin()
        {
            _navigationManager.NavigateTo(TokenEndpoints.KakaoSignin, forceLoad: true);
        }
    }
}