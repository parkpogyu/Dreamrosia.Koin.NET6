using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
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
    public partial class SymbolTable : IDisposable
    {
        [CascadingParameter(Name = "Symbols")]
        private IEnumerable<SymbolDto> Symbols
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }

        [Parameter] public SymbolTableMode TableMode { get; set; } = SymbolTableMode.Full;

        private IEnumerable<SymbolDto> _items { get; set; } = new List<SymbolDto>();
        private IEnumerable<SymbolDto> _sources { get; set; }
        private string _selectedWeekly { get; set; }
        private IEnumerable<string> _selectedWeeklys { get; set; }
        private string _selectedDaily { get; set; }
        private IEnumerable<string> _selectedDailys { get; set; }
        private string _searchString { get; set; } = string.Empty;
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();
        private int _rowsPerPage { get; set; } = 25;

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

                if (!isRendered) { return; }

                _isDivTableRendered = isRendered;

                await SetDivHeightAsync();
            }
            catch (Exception)
            {
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            if (BoundingClientRect.IsMatchMimimumHeight(window.Height))
            {
                var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

                _divTableHeight = $"{divHeight}px";
            }
            else
            {
                _divTableHeight = "auto";
            }

            StateHasChanged();
        }

        private void SetItems()
        {
            _items = _sources.Where(f => (_selectedWeeklys is null ? true :
                                          _selectedWeeklys.Any() ? _selectedWeeklys.Contains(f.WeeklySignal.ToDescriptionString()) : true) &&
                                         (_selectedDailys is null ? true :
                                          _selectedDailys.Any() ? _selectedDailys.Contains(f.DailySignal.ToDescriptionString()) : true)).ToArray();
        }

        public static string GetSeasonSignalIcon(SeasonSignals signal)
        {
            return signal == SeasonSignals.Equal ? "fas fa-equals" :
                   signal == SeasonSignals.Below ? "fas fa-snowflake @Icons.Filled.DownhillSkiing" :
                   signal == SeasonSignals.GoldenCross ? "fas fa-seedling" :
                   signal == SeasonSignals.Above ? "fas fa-sun" :
                   signal == SeasonSignals.DeadCross ? "fas fa-leaf" : "fas fa-question";
        }

        public static readonly string _colorSpring = "#00A845";
        public static readonly string _colorSummer = "OrangeRed";
        public static readonly string _colorAutumn = "Maroon";
        public static readonly string _colorWinter = "#3B3B3B";

        public static string GetSeasonSignalStyle(SeasonSignals signal)
        {
            return string.Format("text-align:center;color:{0}", signal == SeasonSignals.Equal ? "GreenYellow" :
                                                                signal == SeasonSignals.Below ? _colorWinter :
                                                                signal == SeasonSignals.GoldenCross ? _colorSpring :
                                                                signal == SeasonSignals.Above ? _colorSummer :
                                                                signal == SeasonSignals.DeadCross ? _colorAutumn : "Purple");
        }

        private void WeeklySignalSelectionChanged(IEnumerable<string> values)
        {
            _selectedWeeklys = values;

            SetItems();
        }

        private void DailySignalSelectionChanged(IEnumerable<string> values)
        {
            _selectedDailys = values;

            SetItems();
        }

        private string GetSignalSelectionText(List<string> values)
        {
            var count = values.Count();

            if (count == 0)
            {
                return string.Empty;
            }
            else if (count == 1)
            {
                return $"{values.First()}";
            }
            else
            {
                return $"{values.First()}, ...";
            }
        }

        private bool Search(SymbolDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.code?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.korean_name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                if (!_isDivTableRendered) { return; }

                await SetDivHeightAsync();
            });
        }

        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }

        public enum SymbolTableMode
        {
            Full,
            TradingTerms,
            Unpositions
        }
    }
}