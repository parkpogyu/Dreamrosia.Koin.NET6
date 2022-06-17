using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Routes;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Authentication
{
    public partial class Login
    {
        protected override async Task OnInitializedAsync()
        {
            var user = _authenticationManager.CurrentUser();

            var id = user.GetUserId();

            if (string.IsNullOrEmpty(id))
            {
                var result = await _authenticationManager.KakaoLogin();

                if (result.Succeeded)
                {
                    _navigationManager.NavigateTo("/");
                }
            }
            else
            {
                if (user.Identity?.IsAuthenticated == true)
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