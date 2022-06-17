using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class PositionTable : IDisposable
    {
        [CascadingParameter(Name = "Positions")]
        private IEnumerable<PaperPositionDto> Positions
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        private IEnumerable<PaperPositionDto> _sources { get; set; }
        private IEnumerable<PaperPositionDto> _items { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();
        private bool? _chkIsListed { get; set; } = true;
        private string _searchString { get; set; } = string.Empty;
        private int _rowsPerPage { get; set; } = 25;

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

        private void SetItems()
        {
            _items = _sources.Where(f => _chkIsListed is null ? true : f.IsListed == (bool)_chkIsListed).ToArray();
        }

        private void CheckIsListedChanged(bool? value)
        {
            _chkIsListed = value;

            SetItems();
        }

        private bool Search(PaperPositionDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return item.code?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ? true : false;
        }

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
