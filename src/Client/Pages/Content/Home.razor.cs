using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Content
{
    public partial class Home
    {
        [Inject] private IMarketManager MarketManager { get; set; }
        [Inject] private IUPbitKeyManager UPbitKeyManager { get; set; }

        private bool _loaded;
        private IEnumerable<SymbolDto> _items { get; set; } = new List<SymbolDto>();
        private DateTime _expire_at { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetSymbolsAsync();

            _loaded = true;
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
    }
}