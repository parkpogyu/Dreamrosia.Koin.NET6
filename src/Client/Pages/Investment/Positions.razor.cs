using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Common;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Positions : IDisposable
    {
        [Inject] private IInvestmentManager PositionManager { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private HubConnection _synchronizeHubConnection { get; set; }
        private DateTime _receivedTickerTime { get; set; } = DateTime.Now;
        private bool _isReceiveTicker { get; set; } = false;
        private string _userId { get; set; }
        private IEnumerable<PaperPositionDto> _items { get; set; }
        private IEnumerable<SymbolDto> _unpositions { get; set; }
        private long TotalKRW { get; set; }
        private double TotalAsset { get; set; }
        private double TotalPchsAmt { get; set; }
        private double TotalBalEvalAmt { get; set; }
        private double TotalEvalPnL { get; set; }
        private double TotalPnLRat { get; set; }

        private int _activePanelIndex { get; set; } = 0;

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<PositionProfile>(); }).CreateMapper();

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

            await GetPositionsAsync();

            _loaded = true;

            _synchronizeHubConnection = _synchronizeHubConnection.TryInitializeToSynchronize(_navigationManager, _userId);

            _synchronizeHubConnection.On<TickerDto>(ApplicationConstants.SynchronizeSignalR.ReceiveTicker, (ticker) =>
            {
                var coin = _items.SingleOrDefault(f => f.market.Equals(ticker.market));
                var symbol = _unpositions.SingleOrDefault(f => f.market.Equals(ticker.market));

                if (coin is not null)
                {
                    coin.trade_price = ticker.trade_price;
                }

                if (symbol is not null)
                {
                    symbol.trade_price = ticker.trade_price;
                    symbol.signed_change_rate = ticker.signed_change_rate;
                }

                if (_isReceiveTicker || DateTime.Now.Subtract(_receivedTickerTime).TotalSeconds < 1) { return; }

                _isReceiveTicker = true;

                AdjustPositions();

                _receivedTickerTime = DateTime.Now;

                _isReceiveTicker = false;
            });

            _synchronizeHubConnection.On<string, PositionsDto>(ApplicationConstants.SynchronizeSignalR.ReceivePositions, (userId, positions) =>
            {
                if (userId.Equals(_userId))
                {
                    _items = _mapper.Map<IEnumerable<PaperPositionDto>>(positions.Coins);

                    TotalKRW = positions.KRW is null ? 0 : (long)positions.KRW.balance;

                    if (_unpositions.Any())
                    {
                        _unpositions = (from lt in _unpositions
                                        from rt in _items.Where(f => f.market.Equals(lt.market)).DefaultIfEmpty()
                                        where rt is null
                                        select lt).ToArray();
                    }

                    SetItems();
                }
            });

            if (_synchronizeHubConnection.State == HubConnectionState.Disconnected)
            {
                await _synchronizeHubConnection.StartAsync();
            }
        }

        private async Task GetPositionsAsync()
        {
            var response = await PositionManager.GetPositionsAsync(_userId);

            var position = response.Data ?? new PositionsDto();

            _items = _mapper.Map<IEnumerable<PaperPositionDto>>(position.Coins ?? new List<PaperPositionDto>());
            TotalKRW = (long)position.KRW?.balance;
            _unpositions = position.Unpositions ?? new List<SymbolDto>();

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

        private void SetItems()
        {
            AdjustPositions();
        }

        private void AdjustPositions()
        {
            var valids = _items.Where(f => f.unit_currency.Equals(Currency.Unit.KRW) &&
                                           f.trade_price > 0);

            TotalPchsAmt = valids.Sum(f => f.PchsAmt);
            TotalBalEvalAmt = valids.Sum(f => f.BalEvalAmt);
            TotalEvalPnL = valids.Sum(f => f.EvalPnL);
            TotalPnLRat = (double)Ratio.ToSignedPercentage(TotalEvalPnL, TotalPchsAmt);
            TotalAsset = TotalKRW + TotalBalEvalAmt;

            StateHasChanged();
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                _synchronizeHubConnection.Remove(ApplicationConstants.SynchronizeSignalR.ReceiveTicker);
                _synchronizeHubConnection.Remove(ApplicationConstants.SynchronizeSignalR.ReceivePositions);

                await _synchronizeHubConnection.StopAsync();
            });
        }
    }
}