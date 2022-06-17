using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Shared.Components;
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
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private IEnumerable<UserSummaryDto> _items = new List<UserSummaryDto>();
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddMonths(-6).AddDays(1).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._6M;

        protected override async Task OnInitializedAsync()
        {
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

            _items = response.Data ?? new List<UserSummaryDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}