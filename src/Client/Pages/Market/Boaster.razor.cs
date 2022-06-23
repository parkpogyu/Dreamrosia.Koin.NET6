using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Mappings;
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
        private IEnumerable<FollowerDto> _items = new List<FollowerDto>();
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<UserProfile>(); }).CreateMapper();

            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            await GetBoastersAsync();

            _loaded = true;
        }

        private async Task GetBoastersAsync()
        {
            var response = await _userManager.GetBoastersAsync(_dateRange.Start, _dateRange.End);

            _items = _mapper.Map<IEnumerable<FollowerDto>>(response.Data);

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

            await GetBoastersAsync();
        }
    }
}