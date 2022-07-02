using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Models;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class Users
    {
        [Inject] IResizeService ResizeService { get; set; }
        private bool _loaded;
        private IEnumerable<UserFullInfoDto> _items = new List<UserFullInfoDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;

        private bool _isVisibleTabs { get; set; }
        private MudTabs _tabs { get; set; }
        private List<TabView> _views { get; set; } = new List<TabView>();
        private Guid _subscriptionId;
        private int _activePanelIndex { get; set; } = 0;
        private bool _updatePanelIndex = false;
        private PageModes _pageMode => PageModes.Admin;

        protected override async Task OnInitializedAsync()
        {
            await GetUsersAsync();

            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _subscriptionId = await ResizeService.Subscribe((size) =>
                {
                    _isVisibleTabs = BoundingClientRect.IsMatchMediumBreakPoints(size.Width / 2);

                    InvokeAsync(StateHasChanged);
                }, new ResizeOptions
                {
                    ReportRate = 50,
                    NotifyOnBreakpointOnly = false,
                });

                var size = await ResizeService.GetBrowserWindowSize();

                _isVisibleTabs = BoundingClientRect.IsMatchMediumBreakPoints(size.Width / 2);

                StateHasChanged();
            }

            if (_updatePanelIndex == true)
            {
                _activePanelIndex = _views.Count - 1;
                StateHasChanged();
                _updatePanelIndex = false;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task GetUsersAsync()
        {
            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

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

        public async ValueTask DisposeAsync() => await ResizeService.Unsubscribe(_subscriptionId);

        private class TabView : NavigationItem
        {
            public string Id => $"{UserId}-{URL}";
            public string Title { get; set; }
        }
    }
}