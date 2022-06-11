using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
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
    }
}