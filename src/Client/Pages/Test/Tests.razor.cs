using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Test
{
    public partial class Tests
    {
        [Inject] private ITestManager TestManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private bool _loaded;

        private string _userId;

        protected override async Task OnInitializedAsync()
        {
            var user = await _authenticationManager.CurrentUser();

            _userId = user.GetUserId();

            _loaded = true;

            HubConnection = HubConnection.TryInitialize(_navigationManager);

            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task GetSymbolsAsync()
        {
            var response = await TestManager.GetUPbitSymbolsAsync();

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Symbols:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetCrixesAsync()
        {
            var response = await TestManager.GetUPbitCrixesAsync();

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Cirxes:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetCandlesAsync()
        {
            var response = await TestManager.GetUPbitCandlesAsync();

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Candles:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetPositionsAsync()
        {
            var response = await TestManager.GetUPbitPositionsAsync(_userId);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Positions:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetOrdersAsync()
        {
            var response = await TestManager.GetUPbitOrdersAsync(_userId);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Orders:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetDepositsAsync()
        {
            var response = await TestManager.GetUPbitTransfersAsync(_userId, TransferType.deposit);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Deposits:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetWithdrawsAsync()
        {
            var response = await TestManager.GetUPbitTransfersAsync(_userId, TransferType.withdraw);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("Withdraws:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task GetSeasonSignalsAsync()
        {
            var response = await TestManager.GetSeasonSignalsAsync(_userId);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("SeasonSignals:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task UpdateSeasonSignalsAsync()
        {
            var response = await TestManager.UpdateSeasonSignalsAsync(_userId);

            if (response.Succeeded)
            {
                _snackBar.Add(string.Format("SeasonSignals:{0:N0}", response.Data.Count()), Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}