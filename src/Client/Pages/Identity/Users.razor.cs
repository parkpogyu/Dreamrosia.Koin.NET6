using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Shared.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class Users
    {
        private IEnumerable<UserSummaryDto> _items = new List<UserSummaryDto>();

        private bool _loaded;
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddMonths(-1).AddDays(1).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._1M;

        protected override async Task OnInitializedAsync()
        {
            await GetUsersAsync();

            _loaded = true;
        }

        private async Task GetUsersAsync()
        {
            var response = await _userManager.GetSummariseAsync(_dateRange.Start, _dateRange.End);

            _items = response.Data ?? new List<UserSummaryDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}