using Dreamrosia.Koin.Client.Extensions;
using Microsoft.AspNetCore.Components;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }

        private string _nickName { get; set; }
        private string _profileImage { get; set; }

        protected override void OnInitialized()
        {
            var user = _authenticationManager.CurrentUser();

            _nickName = user.GetNickName();
            _profileImage = user.GetProfileImage();
        }
    }
}