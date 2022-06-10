using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Market
{
    public partial class Candles
    {
        [Inject] private IMarketManager MarketManager { get; set; }

        [Parameter] public string Market { get; set; }

        private bool _loaded;

        private IEnumerable<CandleDto> _sources { get; set; }
        private IEnumerable<CandleDto> _items { get; set; }
        private IEnumerable<SymbolDto> _symbols { get; set; } = new List<SymbolDto>();
        private SymbolDto _symbol { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange(DateTime.Now.AddMonths(-6).AddDays(1).Date, DateTime.Now.Date);
        private DateRangePicker.DateRangeTerms _dateRangeTerm { get; set; } = DateRangePicker.DateRangeTerms._6M;

        private TimeFrames _selectedTimeFrame { get; set; } = TimeFrames.Week;

        protected override async Task OnInitializedAsync()
        {
            await GetSymbolsAsync();
            await GetCandlesAsync();

            _loaded = true;
        }

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            _symbols = response.Data ?? new List<SymbolDto>();

            _symbol = string.IsNullOrEmpty(Market) ?
                     _symbols.FirstOrDefault() :
                     _symbols.SingleOrDefault(f => f.market.Equals(Market));

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task GetCandlesAsync()
        {
            var response = await MarketManager.GetCandlesAsync(_symbol is null ? "KRW-BTC" : _symbol.market,
                                                               Convert.ToDateTime(_dateRange.Start),
                                                               Convert.ToDateTime(_dateRange.End));
            _sources = response.Data ?? new List<CandleDto>();

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
            if (_sources is null) { return; }

            _items = _sources.GetTimeFrameCandles(_selectedTimeFrame);

            StateHasChanged();
        }

        private void TimeFrameSelectionChanged(IEnumerable<TimeFrames> values)
        {
            SetItems();
        }

        private async Task<IEnumerable<SymbolDto>> AutoCompleteSearch(string value)
        {
            await Task.Delay(10);

            return _symbols.Where(f => string.IsNullOrEmpty(value) ? true :
                                       f.code.Contains(value, StringComparison.OrdinalIgnoreCase) || f.korean_name.Contains(value)).ToArray();
        }
    }
}