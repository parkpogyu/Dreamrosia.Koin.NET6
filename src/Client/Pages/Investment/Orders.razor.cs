﻿using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
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

        private IMapper _mapper;

        private bool _loaded;
        private string _userId { get; set; }
        private IEnumerable<PaperOrderDto> _items { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddDays(-6).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._1W;

        private readonly OrdersRequestDto _model = new OrdersRequestDto();

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<OrderProfile>(); }).CreateMapper();

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
    }
}

