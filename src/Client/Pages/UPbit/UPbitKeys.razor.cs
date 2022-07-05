using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
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

namespace Dreamrosia.Koin.Client.Pages.UPbit
{
    public partial class UPbitKeys : IAsyncDisposable
    {
        [Inject] private IUPbitKeyManager UPbitKeyManager { get; set; }

        private bool _loaded { get; set; } = false;
        private IEnumerable<UPbitKeyDto> _items { get; set; }
        private IEnumerable<UPbitKeyDto> _sources { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private bool? _isOccurredFatalError { get; set; } = null;
        private string _searchString { get; set; } = string.Empty;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnInitializedAsync()
        {
            if (!_stateProvider.IsInRole(RoleConstants.AdministratorRole))
            {
                _snackBar.Add(_localizer["You are not Authorized."], Severity.Error);
                _navigationManager.NavigateTo("/");
                return;
            }

            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            await GetUPbitKeysAsync();

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

        private async Task GetUPbitKeysAsync()
        {
            var response = await UPbitKeyManager.GetUPbitKeysAsync(_dateRange.Start, _dateRange.End);

            _sources = response.Data ?? new List<UPbitKeyDto>();

            SetItems();

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            if (_dateRangeTerm == value) { return; }

            _dateRangeTerm = value;

            await GetUPbitKeysAsync();
        }

        private void CheckAssignedBotChanged(bool? value)
        {
            _isOccurredFatalError = value;

            SetItems();
        }

        private void SetItems()
        {
            _items = _sources.Where(f => (_isOccurredFatalError is null ? true : f.IsOccurredFatalError == (bool)_isOccurredFatalError))
                             .ToArray();
        }

        private bool Search(UPbitKeyDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            if (item.User.UserCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                item.User.NickName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                item.User.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                item.User.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                item.FatalError?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) { return true; }

            return false;
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}