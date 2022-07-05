using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class AssetTable : IAsyncDisposable
    {
        [CascadingParameter(Name = "Assets")]
        private IEnumerable<AssetDto> Assets
        {
            get => _items;
            set { _items = value; }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        private IEnumerable<AssetDto> _items { get; set; }

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
                var divHeight = (window.Height - rect.Top - 52 - 8);

                _divTableHeight = $"{divHeight}px";
            }
            else
            {
                _divTableHeight = "auto";
            }

            StateHasChanged();
        }

        private async Task AssetsToExcelAsync()
        {
            try
            {
                var data = await _excelService.ExportAsync(_items,
                    sheetName: _localizer["Assets"],
                    mappers: new Dictionary<string, Func<AssetDto, object>>
                    {
                        { _localizer["Date"], item=>item.created_at.Date},
                        { _localizer["Asset.InAmt"], item=>item.InAmt},
                        { _localizer["Asset.OutAmt"], item=>item.OutAmt},
                        { _localizer["Asset.BidAmt"], item=>item.BidAmt},
                        { _localizer["Asset.AskAmt"], item=>item.AskAmt},
                        { _localizer["Asset.Fee"], item=>item.Fee},
                        { _localizer["Asset.PnL"], item=>item.PnL},
                        { _localizer["Asset.BalEvalAmt"], item=>item.BalEvalAmt},
                        { _localizer["Asset.Deposit"], item=>item.Deposit},
                        { _localizer["Asset.DssAmt"], item=>item.DssAmt},
                        { _localizer["Asset.InvsAmt"], item=>item.InvsAmt},
                        { _localizer["Asset.PnLRat"], item=>item.InvsPnLRat}
                    });

                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = data,
                    FileName = $"{_localizer["Assets"]}.xlsx",

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
