using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.BackTesting
{
    public partial class BackTesting
    {
        [Inject] private IBackTestingManager BackTestingManager { get; set; }
        [Inject] private IMarketManager MarketManager { get; set; }

        [CascadingParameter(Name = "ViewHelp")]
        private bool _viewHelp { get; set; }

        private readonly BackTestingRequestDto _model = new BackTestingRequestDto();
        private BackTestingReportDto _report { get; set; } = new BackTestingReportDto();
        private IEnumerable<AssetExtensionDto> _assets { get; set; } = new List<AssetExtensionDto>();
        private IEnumerable<MarketIndexDto> _indices { get; set; }
        private IEnumerable<SymbolDto> _symbols { get; set; } = new List<SymbolDto>();
        private static string _hidden => "Visibility:hidden";
        private bool _isProcessing { get; set; } = false;
        private string _progressBarDisplay { get; set; } = _hidden;

        protected override async Task OnInitializedAsync()
        {
            await GetSymbolsAsync();
        }

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            _symbols = response.Data ?? new List<SymbolDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task GetBackTestingAsync()
        {
            _isProcessing = true;
            _progressBarDisplay = "";

            StateHasChanged();

            var response = await BackTestingManager.GetBackTestingAsync(_model);

            if (response.Succeeded)
            {
                _report = await ObjectGZip.DecompressAsync<BackTestingReportDto>(response.Data);

                await GetMarketIndicesAsync();

                _assets = (from asset in _report.Assets
                           from index in _indices.Where(f => f.candleDateTimeUtc == asset.created_at.Date).DefaultIfEmpty()
                           select ((Func<AssetExtensionDto>)(() =>
                           {
                               var item = _mapper.Map<AssetExtensionDto>(asset);

                               item.index = index?.tradePrice;

                               return item;
                           }))()).ToArray();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _progressBarDisplay = _hidden;
            _isProcessing = false;

            StateHasChanged();
        }

        private  async  Task  GetMarketIndicesAsync()
        {
            var rear = _report.Assets.Min(f => f.created_at);
            var head = _report.Assets.Max(f => f.created_at);

            var response = await MarketManager.GetMarketIndicesAsync(rear, head);

            _indices = response.Data ?? new List<MarketIndexDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}