using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Personal
{
    public partial class Points
    {
        [Inject] private ISettlementManager SettlmentManager { get; set; }

        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private IEnumerable<PointDto> _items { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddMonths(-1).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._1M;

        private readonly PointsRequestDto _model = new PointsRequestDto();

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                var user = await _authenticationManager.CurrentUser();

                _userId = user.GetUserId();
            }
            else
            {
                var isAdmin = _stateProvider.IsAdministrator();

                if (!isAdmin)
                {
                    _snackBar.Add(_localizer["You are not Authorized."], Severity.Error);
                    _navigationManager.NavigateTo("/");
                    return;
                }

                _userId = UserId;
            }

            _model.UserId = _userId;

            await GetPointsAsync();

            _loaded = true;
        }

        private async Task GetPointsAsync()
        {
            _model.HeadDate = Convert.ToDateTime(_dateRange.Start);
            _model.RearDate = Convert.ToDateTime(_dateRange.End);

            var response = await SettlmentManager.GetPointsAsync(_model);

            _items = response.Data ?? new List<PointDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}

