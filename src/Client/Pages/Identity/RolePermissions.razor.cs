using AutoMapper;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Roles;
using Dreamrosia.Koin.Client.Infrastructure.Mappings;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class RolePermissions
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }

        private bool _loaded;
        private ClaimsPrincipal _user { get; set; }
        private PermissionResponse _model;
        private Dictionary<string, List<RoleClaimResponse>> GroupedRoleClaims { get; } = new();
        private RoleClaimResponse _roleClaims = new();
        private RoleClaimResponse _selectedItem = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = true;
        private bool _canEditRolePermissions;

        protected override async Task OnInitializedAsync()
        {
            _user = _authenticationManager.CurrentUser();

            _canEditRolePermissions = (await _authorizationService.AuthorizeAsync(_user, Permissions.RoleClaims.Edit)).Succeeded;

            await GetRolePermissionsAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetRolePermissionsAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<RoleProfile>(); }).CreateMapper();
            var roleId = Id;
            var result = await RoleManager.GetPermissionsAsync(roleId);
            if (result.Succeeded)
            {
                _model = result.Data;
                GroupedRoleClaims.Add(_localizer["All Permissions"], _model.RoleClaims);
                foreach (var claim in _model.RoleClaims)
                {
                    if (GroupedRoleClaims.ContainsKey(claim.Group))
                    {
                        GroupedRoleClaims[claim.Group].Add(claim);
                    }
                    else
                    {
                        GroupedRoleClaims.Add(claim.Group, new List<RoleClaimResponse> { claim });
                    }
                }
                if (_model != null)
                {
                    Title = string.Format("{0} {1}", _model.RoleName, _localizer["Manage Permission"]);
                }
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
                _navigationManager.NavigateTo("/identity/roles");
            }
        }

        private async Task SaveAsync()
        {
            var request = _mapper.Map<PermissionResponse, PermissionRequest>(_model);
            var result = await RoleManager.UpdatePermissionsAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                await HubConnection.SendAsync(ApplicationConstants.SignalR.SendRegenerateTokens);
                await HubConnection.SendAsync(ApplicationConstants.SignalR.OnChangeRolePermissions, _user.GetUserId(), request.RoleId);
                _navigationManager.NavigateTo("/identity/roles");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private bool Search(RoleClaimResponse roleClaims)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (roleClaims.Value?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (roleClaims.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }

        private Color GetGroupBadgeColor(int selected, int all)
        {
            if (selected == 0)
                return Color.Error;

            if (selected == all)
                return Color.Success;

            return Color.Info;
        }
    }
}