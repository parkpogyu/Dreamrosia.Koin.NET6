using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Market
{
    public partial class Symbols
    {
        [Inject] private IMarketManager MarketManager { get; set; }

        private bool _loaded;
        private IEnumerable<SymbolDto> _items { get; set; } = new List<SymbolDto>();

        protected override async Task OnInitializedAsync()
        {
            await GetSymbolsAsync();

            _loaded = true;
        }

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            _items = response.Data ?? new List<SymbolDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private void OnItemsChanged(IEnumerable<SymbolDto> symbols)
        {
            _items = symbols;

            StateHasChanged();
        }
    }
}