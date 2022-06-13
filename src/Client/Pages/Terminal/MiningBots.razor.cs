using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Terminal
{
    public partial class MiningBots : IDisposable
    {
        [Inject] private IMiningBotManager MiningBotManager { get; set; }

        private IEnumerable<MiningBotDto> _items { get; set; } = new List<MiningBotDto>();

        private bool _loaded;
        private string _searchString = "";
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnInitializedAsync()
        {
            await GetMiningBotsAsync();

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

        private async Task GetMiningBotsAsync()
        {
            var response = await MiningBotManager.GetMiningBotsAsync();

            _items = response.Data ?? new List<MiningBotDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private bool Search(MiningBotDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.Ticket?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Id?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.NickName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.MachineName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Version?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.CurrentDirectory?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
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