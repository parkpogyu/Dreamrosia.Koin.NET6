using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Models;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Domain.Enums;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class Users : IAsyncDisposable
    {
        private bool _loaded;
        private IEnumerable<UserFullInfoDto> _items = new List<UserFullInfoDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;

        private static string split_horizontal => "d-flex flex-row";
        private static string split_vertical => "d-flex flex-column";
        private readonly string _splitterId = Guid.NewGuid().ToString();
        private readonly string _leftId = Guid.NewGuid().ToString();
        private bool _isSplitterRendered { get; set; }
        private bool _isSplitHorizontal { get; set; }
        private string _splitClass { get; set; } = split_horizontal;
        private string _dataDirection { get; set; } = "horizontal";

        private MudTabs _tabs { get; set; }
        private List<TabView> _views { get; set; } = new List<TabView>();
        private int _activePanelIndex { get; set; } = 0;
        private bool _updatePanelIndex = false;
        private PageModes _pageMode => PageModes.Admin;

        private Guid _resizeSubscribedId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("LoadScript", "js/splitter.js");

            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            await GetUsersAsync();

            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _resizeSubscribedId = await _resizeService.Subscribe(async (size) =>
                {
                    var breakPoints = BoundingClientRect.IsMatchMediumBreakPoints(size.Width / 2);

                    if (_isSplitHorizontal == breakPoints) { return; }

                    _isSplitHorizontal = breakPoints;

                    _splitClass = _isSplitHorizontal ? split_horizontal : split_vertical;
                    _dataDirection = _isSplitHorizontal ? "horizontal" : "vertical";

                    await InvokeAsync(StateHasChanged);

                    if (!_isSplitHorizontal)
                    {
                        await _jsRuntime.InvokeVoidAsync("func_setWidth", new object[] { _leftId, "inherit" });
                    }

                    await _jsRuntime.InvokeVoidAsync("func_makeResizer", _splitterId);

                }, new ResizeOptions
                {
                    NotifyOnBreakpointOnly = false,
                });

                var size = await _resizeService.GetBrowserWindowSize();

                _isSplitHorizontal = BoundingClientRect.IsMatchMediumBreakPoints(size.Width / 2);

                _splitClass = _isSplitHorizontal ? split_horizontal : split_vertical;
                _dataDirection = _isSplitHorizontal ? "horizontal" : "vertical";

                StateHasChanged();
            }
            else
            {
                if (_updatePanelIndex == true)
                {
                    _activePanelIndex = _views.Count - 1;
                    StateHasChanged();
                    _updatePanelIndex = false;
                }

                if (_isSplitterRendered) { return; }

                var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _splitterId);

                if (!isRendered) { return; }

                _isSplitterRendered = isRendered;

                await _jsRuntime.InvokeVoidAsync("func_makeResizer", _splitterId);
            }

            //await base.OnAfterRenderAsync(firstRender);
        }

        private async Task GetUsersAsync()
        {
            var response = await _userManager.GetFullInfosAsync(_dateRange.Start, _dateRange.End);

            _items = response.Data ?? new List<UserFullInfoDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            if (_dateRangeTerm == value) { return; }

            _dateRangeTerm = value;

            await GetUsersAsync();
        }

        private void NavigationItemSelected(NavigationItem value)
        {
            TabView view = new TabView()
            {
                UserId = value.UserId,
                NickName = value.NickName,
                ProfileImage = value.ProfileImage,
                URL = value.URL,
            };

            var tab = _views.SingleOrDefault(x => x.Id.Equals(view.Id));

            if (tab is not null)
            {
                _tabs.ActivatePanel(_tabs.Panels.Single(f => f.Tag.Equals(tab.Id)));

                return;
            }

            if (view.URL.Equals("/personal/subscription"))
            {
                view.Title = _localizer["Subscriptions"];
            }
            else if (view.URL.Equals("/personal/upbitkey"))
            {
                view.Title = _localizer["UPbit.AuthKey"];
            }
            else if (view.URL.Equals("/personal/followers"))
            {
                view.Title = _localizer["Follower"];
            }
            else if (view.URL.Equals("/personal/points"))
            {
                view.Title = _localizer["Points"];
            }
            else if (view.URL.Equals("/investment/positions"))
            {
                view.Title = _localizer["Positions"];
            }
            else if (view.URL.Equals("/investment/orders"))
            {
                view.Title = _localizer["Orders"];
            }
            else if (view.URL.Equals("/investment/transfers"))
            {
                view.Title = _localizer["Transfers"];
            }
            else if (view.URL.Equals("/investment/assets"))
            {
                view.Title = _localizer["Assets"];
            }
            else if (view.URL.Equals("/order/tradingterms"))
            {
                view.Title = _localizer["TradingTerms"];
            }

            _views.Add(view);
            _updatePanelIndex = true;
        }

        private void RemoveTab(MudTabPanel tabPanel)
        {
            var tab = _views.SingleOrDefault(x => x.Id == (string)tabPanel.Tag);

            if (tab != null)
            {
                _views.Remove(tab);
            }
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);

        private class TabView : NavigationItem
        {
            public string Id => $"{UserId}-{URL}";
            public string Title { get; set; }
        }
    }
}