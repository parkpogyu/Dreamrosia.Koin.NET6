using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Personal
{
    public partial class Followers
    {
        [Parameter] public PageModes PageMode { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private IEnumerable<FollowerDto> _items = new List<FollowerDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private readonly OrdersRequestDto _model = new OrdersRequestDto();

        protected override async Task OnInitializedAsync()
        {
            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            if (string.IsNullOrEmpty(UserId))
            {
                _userId = _authenticationManager.CurrentUser().GetUserId();
            }
            else
            {
                if (!_stateProvider.IsInRole(RoleConstants.AdministratorRole))
                {
                    _snackBar.Add(_localizer["You are not Authorized."], Severity.Error);
                    _navigationManager.NavigateTo("/");
                    return;
                }

                _userId = UserId;
            }

            await GetFollowersAsync();

            _loaded = true;
        }

        private async Task GetFollowersAsync()
        {
            var response = await _userManager.GetFollowersAsync(_userId, _dateRange.Start, _dateRange.End);

            _items = response.Data ?? new List<FollowerDto>();

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

            await GetFollowersAsync();
        }
    }
}