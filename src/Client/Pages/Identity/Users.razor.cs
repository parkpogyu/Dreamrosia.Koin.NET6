﻿using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Domain.Enums;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class Users
    {
        private bool _loaded;
        private IEnumerable<UserFullInfoDto> _items = new List<UserFullInfoDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;

        protected override async Task OnInitializedAsync()
        {
            await GetUsersAsync();

            _loaded = true;
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
    }
}