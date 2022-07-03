using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class OrderTable
    {
        [CascadingParameter(Name = "Orders")]
        private IEnumerable<PaperOrderDto> Orders
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        private IEnumerable<PaperOrderDto> _sources { get; set; }
        private IEnumerable<PaperOrderDto> _items { get; set; }
        private string _selectedOrderSide { get; set; }
        private IEnumerable<string> _selectedOrderSides { get; set; }
        private string _selectedCurrency { get; set; }
        private IEnumerable<string> _selectedCurrencies { get; set; }
        private string _searchString { get; set; } = string.Empty;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

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

        private void SetItems()
        {
            _items = _sources.Where(f => (_selectedCurrencies is null ? true :
                                          _selectedCurrencies.Any() ?
                                          _selectedCurrencies.Contains(f.unit_currency) : true) &&
                                         (_selectedOrderSides is null ? true :
                                          _selectedOrderSides.Any() ?
                                          _selectedOrderSides.Contains(f.side.ToDescriptionString()) : true)
                                         ).ToArray();
        }

        private void CurrencySelectionChanged(IEnumerable<string> values)
        {
            _selectedCurrencies = values;

            SetItems();
        }

        private string GetCurrencySelectionText(List<string> values)
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

        private void OrderSideSelectionChanged(IEnumerable<string> values)
        {
            _selectedOrderSides = values;

            SetItems();
        }

        private string GetOrderSideSelectionText(List<string> values)
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

        private bool Search(PaperOrderDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.code?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Remark?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.ConvertedState.ToDescriptionString().Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        private async Task OrdersToExcelAsync()
        {
            try
            {
                var data = await _excelService.ExportAsync(_sources,
                    sheetName: _localizer["Orders"],
                    mappers: new Dictionary<string, Func<PaperOrderDto, object>>
                    {
                        { _localizer["Date"] ,item=>item.created_at.Date },
                        { _localizer["Symbol.code"] ,item=>item.code },
                        { _localizer["Order.side"] ,item=>item.side.ToDescriptionString() },
                        { _localizer["Order.executed_volume"] ,item=>item.executed_volume },
                        { _localizer["Order.avg_price"] ,item=>item.avg_price },
                        { _localizer["Order.exec_amount"] ,item=>item.exec_amount },
                        { _localizer["Order.paid_fee"] ,item=>item.paid_fee },
                        { _localizer["Order.PnL"] ,item=>item.PnL },
                        { _localizer["Order.PnLRat"] ,item=>item.PnLRat },
                        { _localizer["Remark"] ,item=>item.Remark },
                    });

                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = data,
                    FileName = $"{_localizer["Orders"]}.xlsx",

                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            }
            catch (Exception)
            {
            }
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}
