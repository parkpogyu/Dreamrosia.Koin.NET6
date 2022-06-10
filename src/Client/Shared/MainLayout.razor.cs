using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers.Identity.Roles;
using Dreamrosia.Koin.Client.Infrastructure.Settings;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        private string _userId { get; set; }

        private bool _isLoaded { get; set; }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                if (DateTimeExtensions.Expired(user.GetExpire()))
                {
                    await _authenticationManager.Logout();
                }
                else
                {
                    if (string.IsNullOrEmpty(_userId))
                    {
                        _userId = user.GetUserId();

                        await _hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, _userId);
                    }
                }
            }
            else
            {
                await _authenticationManager.Logout();
            }
        }

        private MudTheme _currentTheme;
        private bool _drawerOpen = true;
        private bool _viewHelp { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            _currentTheme = BlazorHeroTheme.DefaultTheme;
            _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();

            _interceptor.RegisterEvent();

            _hubConnection = _hubConnection.TryInitialize(_navigationManager);

            await _hubConnection.StartAsync();

            _hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
            {
                try
                {
                    var token = await _authenticationManager.TryForceRefreshToken();

                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                    await _authenticationManager.Logout();
                    _navigationManager.NavigateTo("/");
                }
            });

            _hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
            {
                if (_userId != userId)
                {
                    var rolesResponse = await RoleManager.GetRolesAsync();

                    if (rolesResponse.Succeeded)
                    {
                        var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);

                        if (role != null)
                        {
                            var currentUserRolesResponse = await _userManager.GetRolesAsync(_userId);

                            if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                            {
                                _snackBar.Add(_localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);

                                await _hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, _userId);
                                await _authenticationManager.Logout();

                                _navigationManager.NavigateTo("/login");
                            }
                        }
                    }
                }
            });
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{_localizer["Do you want Logout?"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{_localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), _userId},
                {nameof(Dialogs.Logout.HubConnection), _hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Dialogs.Logout>(_localizer["Logout"], parameters, options);
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task DarkMode()
        {
            bool isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();

            _currentTheme = isDarkMode ?
                            BlazorHeroTheme.DefaultTheme :
                            BlazorHeroTheme.DarkTheme;
        }

        public void Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private HubConnection _hubConnection;
        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;
    }
}