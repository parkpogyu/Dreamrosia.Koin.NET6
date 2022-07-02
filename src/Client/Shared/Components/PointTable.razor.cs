using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class PointTable : IDisposable
    {
        [CascadingParameter(Name = "Points")]
        private IEnumerable<PointDto> Points
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        private IEnumerable<PointDto> _sources { get; set; }
        private IEnumerable<PointDto> _items { get; set; }
        private PointDto _selectedItem { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();
        private string _selectedPointType { get; set; }
        private IEnumerable<string> _selectedPointTypes { get; set; }
        private string _searchString { get; set; } = string.Empty;

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
            _items = _sources.Where(f => (_selectedPointTypes is null ? true :
                                          _selectedPointTypes.Any() ?
                                          _selectedPointTypes.Contains(f.Type.ToDescriptionString()) : true)).ToArray();
        }

        private void PointTypeSelectionChanged(IEnumerable<string> values)
        {
            _selectedPointTypes = values;

            SetItems();
        }

        private string GetPointTypeSelectionText(List<string> values)
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

        private bool Search(PointDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.Transaction.Contents?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Transaction.Counterparty?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Membership.ToDescriptionString().Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        //private async Task PointsToExcelAsync()
        //{
        //    try
        //    {
        //        var data = await _excelService.ExportAsync(_points,
        //            sheetName: _localizer["Points"],
        //            mappers: new Dictionary<string, Func<PointDto, object>>
        //            {
        //                { _localizer["Point.done_at"] ,item=>item.done_at.Date },
        //                { _localizer["Point.Classification"] ,item=>item.Classification},
        //                { _localizer["Point.Contents"] ,item=>item.Contents },
        //                { _localizer["Point.Deposit"] ,item=>item.Deposit },
        //                { _localizer["Point.Withdraw"] ,item=>item.Withdraw },
        //                { _localizer["Point.Balance"] ,item=>item.Balance },
        //                { _localizer["Point.Counterparty"] ,item=>item.Counterparty },
        //                { _localizer["Point.Memo"] ,item=>item.Memo },
        //                { _localizer["Subscription.UserCode"] ,item=>item.UserCode },
        //            });

        //        await _jsRuntime.InvokeVoidAsync("Download", new
        //        {
        //            ByteArray = data,
        //            FileName = $"{_localizer["Settlement.Points"]}.xlsx",

        //            MimeType = ApplicationConstants.MimeTypes.OpenXml
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                if (_isDivTableRendered)
                {
                    await SetDivHeightAsync();
                }
            });
        }

        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }
    }
}
