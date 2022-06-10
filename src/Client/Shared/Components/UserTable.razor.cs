using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class UserTable : IDisposable
    {
        [CascadingParameter(Name = "Users")]
        private IEnumerable<UserSummaryDto> Users
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        [Parameter] public UserTableMode TableMode { get; set; } = UserTableMode.Admin;

        private MudTable<UserSummaryDto> _table;


        private ClaimsPrincipal _currentUser;
        private bool _canViewRoles;

        private IEnumerable<UserSummaryDto> _items = new List<UserSummaryDto>();
        private IEnumerable<UserSummaryDto> _sources;

        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        private string _searchString { get; set; } = string.Empty;

        private bool? _chkIsAssignedBot { get; set; } = null;
        private bool? _chkIsAutoTrading { get; set; } = null;
        private string _selectedTimeFrame { get; set; }
        private IEnumerable<string> _selectedTimeFrames { get; set; }
        private string _selectedMembership { get; set; }
        private IEnumerable<string> _selectedMemberships { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View)).Succeeded;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeListener.OnResized += OnWindowResized;
                }

                if (_isDivTableRendered) { return; }

                var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                if (!isRendered) { return; }

                _isDivTableRendered = isRendered;

                await SetDivHeightAsync();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

            _divTableHeight = $"{divHeight}px";

            StateHasChanged();
        }

        private void SetItems()
        {
            _items = _sources.Where(f => (_selectedMemberships is null ? true :
                                          _selectedMemberships.Any() ? _selectedMemberships.Contains(f.MembershipLevel.ToDescriptionString()) : true) &&
                                         (_chkIsAutoTrading is null ? true : f.AutoTrading == (bool)_chkIsAutoTrading) &&
                                         (_selectedTimeFrames is null ? true :
                                          _selectedTimeFrames.Any() ? _selectedTimeFrames.Contains(f.TimeFrame.ToDescriptionString()) : true) &&
                                         (_chkIsAssignedBot is null ? true : f.IsAssignedBot == (bool)_chkIsAssignedBot)).ToArray();
        }

        private void MembershipSelectionChanged(IEnumerable<string> values)
        {
            _selectedMemberships = values;

            SetItems();
        }

        private void CheckAutoTradingChanged(bool? value)
        {
            _chkIsAutoTrading = value;

            SetItems();
        }

        private void TimeFrameSelectionChanged(IEnumerable<string> values)
        {
            _selectedTimeFrames = values;

            SetItems();
        }

        private void CheckAssignedBotChanged(bool? value)
        {
            _chkIsAssignedBot = value;

            SetItems();
        }

        private string GetSelectionText(List<string> values)
        {
            var count = values.Count();

            if (count == 0)
            {
                return string.Empty;
            }
            else if (count == 1)
            {
                return $"{values.First()}";
            }
            else
            {
                return $"{values.First()}, ...";
            }
        }

        private bool Search(UserSummaryDto user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            if (user.UserCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.NickName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                ) { return true; }

            return false;
        }


        private void ShowBtnPress(string userId)
        {
            var user = _items.Single(f => f.Id.Equals(userId));

            foreach (var item in _items.Where(a => !a.Id.Equals(userId)))
            {
                item.ShowDetails = false;
            }

            user.ShowDetails = !user.ShowDetails;
        }

        private void NavigateToProfile(string userId)
        {
            _navigationManager.NavigateTo($"/personal/subscription/{userId}");
        }

        private Task NavigateToPositions(string userId)
        {
            _navigationManager.NavigateTo($"/investment/positions/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToOrders(string userId)
        {
            _navigationManager.NavigateTo($"/investment/orders/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToTransfers(string userId)
        {
            _navigationManager.NavigateTo($"/investment/transfers/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToAssets(string userId)
        {
            _navigationManager.NavigateTo($"/investment/assets/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToTradingTerms(string userId)
        {
            _navigationManager.NavigateTo($"/order/tradingterms/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToUPbitKey(string userId)
        {
            _navigationManager.NavigateTo($"/personal/upbitkey/{userId}");

            return Task.CompletedTask;
        }

        private Task NavigateToFollowers(string userId)
        {
            _navigationManager.NavigateTo($"/personal/followers/{userId}");

            return Task.CompletedTask;
        }

        private void NavigateToMangeRoles(string userId, string email)
        {
            if (email.Equals("mjtobi@naver.com") || email.Equals("mjtobi@gmail.com"))
            {
                _snackBar.Add(_localizer["Not Allowed."], Severity.Error);
            }
            else
            {
                _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
            }
        }

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                if (!_isDivTableRendered) { return; }

                await SetDivHeightAsync();
            });
        }

        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }

        public enum UserTableMode
        {
            Admin,
            Follower,
            Boast
        }
    }
}
