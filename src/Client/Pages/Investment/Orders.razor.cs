using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Orders
    {
        [Inject] private IInvestmentManager InvestmentManager { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private IEnumerable<PaperOrderDto> _items { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private readonly OrdersRequestDto _model = new OrdersRequestDto();

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<OrderProfile>(); }).CreateMapper();

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

            _model.UserId = _userId;

            await GetOrdersAsync();

            _loaded = true;
        }

        private async Task GetOrdersAsync()
        {
            _model.HeadDate = Convert.ToDateTime(_dateRange.Start);
            _model.RearDate = Convert.ToDateTime(_dateRange.End);

            var response = await InvestmentManager.GetOrdersAsync(_model);

            _items = _mapper.Map<IEnumerable<PaperOrderDto>>(response.Data ?? new List<PaperOrderDto>());

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            _dateRangeTerm = value;

            await GetOrdersAsync();
        }
    }
}

