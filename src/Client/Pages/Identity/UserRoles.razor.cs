using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class UserRoles
    {
        [Parameter] public string Id { get; set; }

        private bool _loaded;
        private ClaimsPrincipal _user { get; set; }
        private bool _canEditUsers;
        public List<UserRoleModel> _items { get; set; } = new();
        private UserRoleModel _item = new();
        private string _searchString = "";

        protected override async Task OnInitializedAsync()
        {
            _user = _authenticationManager.CurrentUser();
            _canEditUsers = (await _authorizationService.AuthorizeAsync(_user, Permissions.Users.Edit)).Succeeded;

            var userId = Id;
            var result = await _userManager.GetUserBriefAsync(userId);

            if (result.Succeeded)
            {
                var user = result.Data;

                if (user != null)
                {
                    var response = await _accountManager.GetRolesAsync(userId);

                    _items = response.Data.UserRoles;
                }
            }

            _loaded = true;
        }

        private async Task SaveAsync()
        {
            var request = new UpdateUserRolesRequest()
            {
                UserId = Id,
                UserRoles = _items
            };
            var result = await _accountManager.UpdateRolesAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/identity/users");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }
        private bool Search(UserRoleModel userRole)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) { return true; }

            if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                userRole.RoleDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            { return true; }

            return false;
        }
    }
}