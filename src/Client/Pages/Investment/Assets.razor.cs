using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Investment
{
    public partial class Assets
    {
        [Inject] private IInvestmentManager InvestmentManager { get; set; }
        [CascadingParameter(Name = "ViewHelp")]
        private bool _viewHelp { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private string _userId { get; set; }
        private DateTime _signUpDate { get; set; }
        private AssetReportDto _report { get; set; } = new();
        private int _activePanelIndex { get; set; } = 0;
        private static string _hidden => "Visibility:hidden";
        private bool _isProcessing { get; set; } = false;
        private string _progressBarDisplay { get; set; } = _hidden;

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("loadScript", "https://html2canvas.hertzen.com/dist/html2canvas.min.js");
            await _jsRuntime.InvokeVoidAsync("loadScript", "js/screenshot.js");

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

            var response = await _userManager.GetDetailAsync(_userId);

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

            if (!response.Succeeded)
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