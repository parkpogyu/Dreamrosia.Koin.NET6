using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Terminal
{
    public partial class MiningBots : IAsyncDisposable
    {
        [Inject] private IMiningBotManager MiningBotManager { get; set; }

        private bool _loaded;
        private IEnumerable<MiningBotTicketDto> _items { get; set; } = new List<MiningBotTicketDto>();
        private IEnumerable<MiningBotTicketDto> _sources { get; set; } = new List<MiningBotTicketDto>();
        private bool? _chkIsAssignedBotTicket { get; set; } = true;
        private string _searchString = "";

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();
        private System.Threading.Timer? _timer;

        protected override async Task OnInitializedAsync()
        {
            await GetMiningBotTicketsAsync();

            _timer = new System.Threading.Timer(async (object? stateInfo) =>
            {
                await GetMiningBotTicketsAsync();

                StateHasChanged(); // NOTE: MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT

            }, new System.Threading.AutoResetEvent(false), 2000, 2000); // fire every 2000 milliseconds

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

        private void SetItems()
        {
            _items = _sources.Where(f => (_chkIsAssignedBotTicket is null ? true : (f.User is not null) == (bool)_chkIsAssignedBotTicket)).ToArray();
        }


        private void CheckAssignedBotTicketChanged(bool? value)
        {
            _chkIsAssignedBotTicket = value;

            SetItems();
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

        private async Task GetMiningBotTicketsAsync()
        {
            var response = await MiningBotManager.GetMiningBotTicketsAsync();

            _sources = response.Data ?? new List<MiningBotTicketDto>();

            SetItems();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private bool Search(MiningBotTicketDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.Id?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.MiningBot?.Id.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.MiningBot?.MachineName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.MiningBot?.Version?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.MiningBot?.CurrentDirectory?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.User?.NickName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        public async ValueTask DisposeAsync()
        {
            _timer?.Dispose();
            await _resizeService.Unsubscribe(_resizeSubscribedId);
        }
    }
}