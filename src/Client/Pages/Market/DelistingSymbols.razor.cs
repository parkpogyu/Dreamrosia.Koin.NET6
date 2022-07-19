using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Market
{
    public partial class DelistingSymbols : IAsyncDisposable
    {
        [Inject] private IMarketManager MarketManager { get; set; }

        private bool _loaded;
        private bool _canEditDelistingSymbols;

        private IEnumerable<DelistingSymbolDto> _items { get; set; } = new List<DelistingSymbolDto>();
        private DelistingSymbolDto _model { get; set; } = new();
        private HashSet<DelistingSymbolDto> _selectedItems { get; set; }
        private IEnumerable<SymbolDto> _symbols { get; set; } = new List<SymbolDto>();
        private SymbolDto _symbol { get; set; }
        private string _searchString { get; set; } = string.Empty;
        private int _rowsPerPage { get; set; } = TablePager.DefaultPageSize;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnInitializedAsync()
        {
            var user = _authenticationManager.CurrentUser();

            _canEditDelistingSymbols = (await _authorizationService.AuthorizeAsync(user, Permissions.Symbols.Edit)).Succeeded;

            await GetSymbolsAsync();
            await GetDelistingSymbolsAsync();

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

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            _symbols = response.Data ?? new List<SymbolDto>();
            _symbol = _symbols.FirstOrDefault();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task GetDelistingSymbolsAsync()
        {
            var response = await MarketManager.GetDelistingSymbolsAsync();

            _items = response.Data ?? new List<DelistingSymbolDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task RegistDelistingSymbolAsync()
        {
            if (_symbol is null) { return; }
            if (Convert.ToDateTime(_model.CloseAt) < Convert.ToDateTime(_model.NotifiedAt)) { return; }

            _model.market = _symbol.market;
            _model.korean_name = _symbol.korean_name;

            var response = await MarketManager.RegistDelistingSymbolAsync(_model);

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, response.Succeeded ? Severity.Success : Severity.Error);
            }

            await GetDelistingSymbolsAsync();
        }

        private async Task DeleteDelistingSymbolsAsync()
        {
            if (_selectedItems is null || !_selectedItems.Any()) { return; }    

            var response = await MarketManager.DeleteDelistingSymbolsAsync(_selectedItems);

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, response.Succeeded ? Severity.Success : Severity.Error);
            }

            await GetDelistingSymbolsAsync();
        }


        private void MarketValueChanged(SymbolDto value)
        {
            _symbol = value;
        }

        private async Task<IEnumerable<SymbolDto>> AutoCompleteSearch(string value)
        {
            await Task.Delay(10);

            return _symbols.Where(f => string.IsNullOrEmpty(value) ? true :
                                       f.code.Contains(value, StringComparison.OrdinalIgnoreCase) || f.korean_name.Contains(value)).ToArray();
        }

        private bool Search(DelistingSymbolDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.market?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.korean_name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }


        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}