using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Transfers : IAsyncDisposable
    {
        [Inject] private IInvestmentManager InvestmentManager { get; set; }
        [Parameter] public PageModes PageMode { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded { get; set; } = false;
        private string _userId { get; set; } = string.Empty;
        private IEnumerable<TransferDto> _items { get; set; }
        private IEnumerable<TransferDto> _sources { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private string _selectedTransferType { get; set; }
        private IEnumerable<string> _selectedTransferTypes { get; set; }
        private string _searchString { get; set; } = string.Empty;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        private readonly TransfersRequestDto _model = new TransfersRequestDto();

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
                    _resizeSubscribedId = await _resizeService.Subscribe((size) =>
                    {
                        if (!_isDivTableRendered) { return; }

                        InvokeAsync(SetDivHeightAsync);

                    }, new ResizeOptions
                    {
                        NotifyOnBreakpointOnly = false,
                    });
                }
                else
                {
                    if (_isDivTableRendered) { return; }

                    var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                    if (!isRendered) { return; }

                    _isDivTableRendered = isRendered;

                    await SetDivHeightAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            if (BoundingClientRect.IsMatchMediumBreakPoints(window.Height))
            {
                var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

                _divTableHeight = $"{divHeight}px";
            }
            else
            {
                _divTableHeight = "auto";
            }

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


        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            if (_dateRangeTerm == value) { return; }

            _dateRangeTerm = value;

            await GetTransfersAsync();
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

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}