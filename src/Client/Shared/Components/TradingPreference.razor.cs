using Blazored.FluentValidation;
using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Dialogs;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class TradingPreference : IDisposable
    {
        [Inject] private ITradingTermsManager TradingTermsManager { get; set; }
        [Inject] private IMarketManager MarketManager { get; set; }

        [CascadingParameter(Name = "ViewHelp")]
        private bool _viewHelp { get; set; }

        [CascadingParameter(Name = "TradingTerms")]
        private BackTestingRequestDto TradingTerms
        {
            get => _model;
            set
            {
                _model = value;

                SetSelectedSymbols();
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        [Parameter]
        public string UserId
        {
            get => _userId;
            set { _userId = value; }
        }

        [Parameter]
        public IEnumerable<SymbolDto> Symbols
        {
            get => _symbols;
            set
            {
                _symbols = value;
            }
        }

        private IEnumerable<SymbolDto> _symbols { get; set; }

        private bool _loaded;

        private FluentValidationValidator _askTermsValidator;
        private bool _askTermsValidated => _askTermsValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private FluentValidationValidator _bidTermsValidator;
        private bool _bidTermsValidated => _bidTermsValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private string _userId { get; set; }

        private BackTestingRequestDto _model { get; set; }

        private HashSet<SymbolDto> _selectedSymbols { get; set; } = new HashSet<SymbolDto>();

        private float _autoAmountRate { get; set; }

        private string _searchString { get; set; } = string.Empty;

        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override void OnInitialized()
        {
            SetSelectedSymbols();
            SetAutoAmountRate();

            _loaded = true;
        }

        private void SetSelectedSymbols()
        {
            if (_symbols is not null && _model is not null)
            {
                _selectedSymbols = (from lt in _symbols
                                    from rt in _model.ChosenSymbols.Where(f => f.Equals(lt.market)).DefaultIfEmpty()
                                    where rt is not null
                                    select lt).ToHashSet();
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeListener.OnResized += OnWindowResized;
                }

                if (_isDivTableRendered) { return; }

                var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                if (isRendered)
                {
                    _isDivTableRendered = isRendered;

                    await SetDivHeightAsync(_divTableId);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task SetDivHeightAsync(string div)
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", div);

            if (rect is null) { return; }

            var divHeight = (window.Height - rect.Top - 52 - 8);

            _divTableHeight = $"{divHeight - 64}px";

            StateHasChanged();
        }

        private void BidAmountOptionChanged(BidAmountOption value)
        {
            _model.AmountOption = value;

            SetAutoAmountRate();

            StateHasChanged();
        }

        private void SetAutoAmountRate()
        {
            if (_model.AmountOption == BidAmountOption.Auto)
            {
                _autoAmountRate = (1F / (_selectedSymbols.Any() ? _selectedSymbols.Count() : _symbols.Count())) * 100F;
            }
        }

        private void SelectedSymbolsChanged(HashSet<SymbolDto> symbols)
        {
            _selectedSymbols = symbols;

            _model.ChosenSymbols = _selectedSymbols.Select(f => f.market).ToArray();

            SetAutoAmountRate();
        }

        private bool Search(SymbolDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.code?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.korean_name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        private async Task UpdateTradingTermsAsync()
        {
            var parameters = new DialogParameters();

            parameters.Add("ContentText", $"{_localizer["Do you want to change trading terms information?"]}");

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };

            var dialog = _dialogService.Show<Confirmation>($"{_localizer["TradingTerms"]} {_localizer["Change"]}", parameters, options);

            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            _model.ChosenSymbols = _selectedSymbols.Select(f => f.market);

            var response = await TradingTermsManager.UpdateTradingTermsAsync(_model);

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, response.Succeeded ? Severity.Success : Severity.Error);
            }
        }


        private async Task<string> ScreenshotAsync()
        {
            string name = $"{_localizer["TradingTerms"]}";

            return await _jsRuntime.InvokeAsync<string>("Screenshot", new object[] { "TradingPreference", name });
        }


        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                await SetDivHeightAsync(_divTableId);
            });
        }
    }
}