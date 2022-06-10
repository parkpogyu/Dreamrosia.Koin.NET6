using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class SymbolsSummary : IDisposable
    {
        [CascadingParameter(Name = "Symbols")]
        public IEnumerable<SymbolDto> Symbols
        {
            get => _items;
            set { _items = value; }
        }

        [Parameter] public Action<IEnumerable<SymbolDto>> OnItemsChanged { get; set; }

        [Inject] private IMarketManager MarketManager { get; set; }

        private bool _loaded;

        private IEnumerable<SymbolDto> _items { get; set; } = new List<SymbolDto>();
        private int _weeklySpring => _items.Any() ? _items.Count(f => f.WeeklySignal == SeasonSignals.GoldenCross) : 0;
        private int _weeklySummer => _items.Any() ? _items.Count(f => f.WeeklySignal == SeasonSignals.Above) : 0;
        private int _weeklyAutumn => _items.Any() ? _items.Count(f => f.WeeklySignal == SeasonSignals.DeadCross) : 0;
        private int _weeklyWinter => _items.Any() ? _items.Count(f => f.WeeklySignal == SeasonSignals.Below) : 0;

        private int _dailySpring => _items.Any() ? _items.Count(f => f.DailySignal == SeasonSignals.GoldenCross) : 0;
        private int _dailySummer => _items.Any() ? _items.Count(f => f.DailySignal == SeasonSignals.Above) : 0;
        private int _dailyAutumn => _items.Any() ? _items.Count(f => f.DailySignal == SeasonSignals.DeadCross) : 0;
        private int _dailyWinter => _items.Any() ? _items.Count(f => f.DailySignal == SeasonSignals.Below) : 0;

        private int _riseCount => _items.Any() ? _items.Count(f => f.signed_change_rate > 0) : 0;
        private int _fallCount => _items.Any() ? _items.Count(f => f.signed_change_rate < 0) : 0;
        private int _evenCount => _items.Any() ? _items.Count(f => f.signed_change_rate == 0) : 0;
        private double _avgChangeRate => _items.Any() ? _items.Average(f => f.signed_change_rate) : 0;

        private DateTime _receivedTickerTime { get; set; } = DateTime.Now;
        private bool _isReceiveTicker { get; set; } = false;

        private HubConnection SynchronizeHubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (_items is null)
            {
                await GetSymbolsAsync();
            }

            _loaded = true;

            //var _currentUser = await _authenticationManager.CurrentUser();

            SynchronizeHubConnection = SynchronizeHubConnection.TryInitializeToSynchronize(_navigationManager, Guid.NewGuid().ToString());

            SynchronizeHubConnection.On<TickerDto>(ApplicationConstants.SynchronizeSignalR.ReceiveTicker, (ticker) =>
            {
                var symbol = _items.SingleOrDefault(f => f.market.Equals(ticker.market));

                if (symbol is null) { return; }

                symbol.trade_price = ticker.trade_price;
                symbol.signed_change_rate = ticker.signed_change_rate;

                if (_isReceiveTicker || DateTime.Now.Subtract(_receivedTickerTime).TotalSeconds < 1) { return; }

                _isReceiveTicker = true;

                StateHasChanged();

                ItemsChanged();

                _receivedTickerTime = DateTime.Now;

                _isReceiveTicker = false;
            });

            if (SynchronizeHubConnection.State == HubConnectionState.Disconnected)
            {
                await SynchronizeHubConnection.StartAsync();
            }
        }

        private void ItemsChanged()
        {
            OnItemsChanged?.Invoke(_items);
        }

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            if (response.Succeeded)
            {
                _items = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                SynchronizeHubConnection.Remove(ApplicationConstants.SynchronizeSignalR.ReceiveTicker);

                await SynchronizeHubConnection.StopAsync();
            });
        }
    }
}