using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Transfers : IDisposable
    {
        [Inject] private IInvestmentManager InvestmentManager { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded { get; set; } = false;
        private string _userId { get; set; } = string.Empty;
        private IEnumerable<TransferDto> _items { get; set; }
        private IEnumerable<TransferDto> _sources { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddMonths(-1).AddDays(1).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._1W;
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();
        private string _selectedTransferType { get; set; }
        private IEnumerable<string> _selectedTransferTypes { get; set; }
        private string _searchString { get; set; } = string.Empty;

        private readonly TransfersRequestDto _model = new TransfersRequestDto();

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

            await GetTransfersAsync();

            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeListener.OnResized += OnWindowResized;
                }

                if (_isDivTableRendered) { return; }

                var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                if (!isRendered) { return; }

                _isDivTableRendered = isRendered;

                await SetDivHeightAsync();
            }
            catch (Exception)
            {
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

            _divTableHeight = $"{divHeight}px";

            StateHasChanged();
        }

        private async Task GetTransfersAsync()
        {
            _model.HeadDate = Convert.ToDateTime(_dateRange.Start);
            _model.RearDate = Convert.ToDateTime(_dateRange.End);

            var response = await InvestmentManager.GetTransfersAsync(_model);

            _sources = response.Data ?? new List<TransferDto>();

            if (response.Succeeded)
            {
                SetItems();

                return;
            }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private void SetItems()
        {
            _items = _sources.Where(f => (_selectedTransferTypes is null ? true :
                                          _selectedTransferTypes.Any() ? _selectedTransferTypes.Contains(f.type.ToDescriptionString()) : true)).ToArray();
        }

        private void TransferTypeSelectionChanged(IEnumerable<string> values)
        {
            _selectedTransferTypes = values;

            SetItems();
        }

        private string GetTransferTypeSelectionText(List<string> values)
        {
            var count = values.Count();

            if (count == 0)
            {
                return string.Empty;
            }
            else if (count == 1)
            {
                return $"{values.First()}";
            }
            else
            {
                return $"{values.First()}, ...";
            }
        }

        private bool Search(TransferDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return item.code?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ? true : false;
        }

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                if (!_isDivTableRendered) { return; }

                await SetDivHeightAsync();
            });
        }

        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }
    }
}