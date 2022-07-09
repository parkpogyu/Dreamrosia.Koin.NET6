using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Assets
    {
        [Inject] private IInvestmentManager InvestmentManager { get; set; }
        [Inject] private IMarketManager MarketManager { get; set; }

        [CascadingParameter(Name = "ViewHelp")]
        private bool _viewHelp { get; set; }
        [Parameter] public PageModes PageMode { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private DateTime _signUpDate { get; set; }
        private AssetReportDto _report { get; set; } = new();
        private IEnumerable<AssetExtensionDto> _assets { get; set; } = new List<AssetExtensionDto>();
        private IEnumerable<MarketIndexDto> _indices { get; set; }
        private int _activePanelIndex { get; set; } = 0;
        private static string _hidden => "Visibility:hidden";
        private bool _isProcessing { get; set; } = false;
        private string _progressBarDisplay { get; set; } = _hidden;

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("LoadScript", "https://html2canvas.hertzen.com/dist/html2canvas.min.js");
            await _jsRuntime.InvokeVoidAsync("LoadScript", "js/screenshot.js");

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

            var response = await _userManager.GetUserBriefAsync(_userId);

            if (response.Succeeded)
            {
                _signUpDate = response.Data.CreatedOn;
            }

            await GetAssetsAsync();

            _loaded = true;
        }

        private async Task GetAssetsAsync()
        {
            _isProcessing = true;
            _progressBarDisplay = "";

            StateHasChanged();

            var response = await InvestmentManager.GetAssetsAsync(_userId);

            _report = response.Data ?? new AssetReportDto();

            if (response.Succeeded)
            {
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

        private async Task GetMarketIndicesAsync()
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

        private async Task<string> ScreenshotAsync()
        {
            string name = $"{_localizer["Assets"]}";

            if (_activePanelIndex == 0)
            {
                name = $"{name}_{_localizer["Asset.Report"]}";
            }
            else if (_activePanelIndex == 2)
            {
                name = $"{name}_{_localizer["Chart"]}";
            }

            return await _jsRuntime.InvokeAsync<string>("Screenshot", new object[] { "Assets", name });
        }
    }
}