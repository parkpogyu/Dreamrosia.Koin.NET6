using Dreamrosia.Koin.Client.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class UserCard
    {
        [Parameter] public string Class { get; set; }
        private string _nickName { get; set; }
        private string _profileImage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var user = await _authenticationManager.CurrentUser();

            if (user is null) { return; }

            _nickName = user.GetNickName();
            _profileImage = user.GetProfileImage();
        }
    }
}