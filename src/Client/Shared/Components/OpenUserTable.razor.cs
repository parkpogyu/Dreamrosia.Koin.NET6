using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class OpenUserTable : IAsyncDisposable
    {
        [CascadingParameter(Name = "Users")]
        private IEnumerable<FollowerDto> Users
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        [Parameter] public TableMode Mode { get; set; } = TableMode.Follower;

        private IEnumerable<FollowerDto> _items = new List<FollowerDto>();
        private IEnumerable<FollowerDto> _sources;
        private string _searchString { get; set; } = string.Empty;
        private bool? _chkIsAssignedBot { get; set; } = null;
        private bool? _chkIsAutoTrading { get; set; } = null;
        private string _selectedTimeFrame { get; set; }
        private IEnumerable<string> _selectedTimeFrames { get; set; }
        private string _selectedMembership { get; set; }
        private IEnumerable<string> _selectedMemberships { get; set; }

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeSubscribedId = await _resizeService.Subscribe((size) =>
                    {
                        if (!_isDivTableRendered) { return; }

                        InvokeAsync(SetDivHeightAsync);

                    }, new ResizeOptions
                    {
                        NotifyOnBreakpointOnly = false,
                    });
                }
                else
                {
                    if (_isDivTableRendered) { return; }

                    var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                    if (!isRendered) { return; }

                    _isDivTableRendered = isRendered;

                    await SetDivHeightAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            if (BoundingClientRect.IsMatchMediumBreakPoints(window.Height))
            {
                var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

                _divTableHeight = $"{divHeight}px";
            }
            else
            {
                _divTableHeight = "auto";
            }

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

        private bool Search(FollowerDto user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            if (user.UserCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.NickName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true
                ) { return true; }

            return false;
        }

        private Task NavigateToAssets(string userId)
        {
            _navigationManager.NavigateTo($"/investment/assets/{userId}");

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);

        public enum TableMode
        {
            Follower,
            Boaster
        }
    }
}
