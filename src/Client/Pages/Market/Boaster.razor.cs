using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Market
{
    public partial class Boaster
    {
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private IEnumerable<UserSummaryDto> _items = new List<UserSummaryDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;

        protected override async Task OnInitializedAsync()
        {
            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            await GetBoastersAsync();

            _loaded = true;
        }

        private async Task GetBoastersAsync()
        {
            var response = await _userManager.GetBoastersAsync(_dateRange.Start, _dateRange.End);

            _items = response.Data ?? new List<UserSummaryDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            _dateRangeTerm = value;

            await GetBoastersAsync();
        }
    }
}