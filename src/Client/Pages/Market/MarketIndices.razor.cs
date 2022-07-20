using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Market
{
    public partial class MarketIndices
    {
        [Inject] private ITradingTermsManager TradingTermsManager { get; set; }
        [Inject] private IMarketManager MarketManager { get; set; }
        [Inject] private IMACDService MACDService { get; set; }

        [Parameter] public string Market { get; set; }

        private bool _loaded;
        private IEnumerable<CandleDto> _sources { get; set; }
        private IEnumerable<CandleExtensionDto> _items { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._3M;
        private TimeFrames _selectedTimeFrame { get; set; } = TimeFrames.Week;

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c =>
            {
                c.AddProfile<MarketIndexProfile>();
                c.AddProfile<CandleProfile>();
            }).CreateMapper();

            var userId = _authenticationManager.CurrentUser().GetUserId();

            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await TradingTermsManager.GetTradingTermsAsync(userId);

                if (result.Succeeded)
                {
                    var terms = result.Data;

                    _selectedTimeFrame = terms.TimeFrame;

                    if (_selectedTimeFrame == TimeFrames.Week)
                    {
                        _dateRangeTerm = DateRangeTerms._6M;
                        _dateRange.Start = now.GetBefore(_dateRangeTerm);
                    }
                }
            }


            await GetMarketIndicesAsync();

            SetItems();

            _loaded = true;
        }

        private async Task GetMarketIndicesAsync()
        {
            var response = await MarketManager.GetMarketIndicesAsync(Convert.ToDateTime(_dateRange.Start),
                                                                     Convert.ToDateTime(_dateRange.End));

            _sources = _mapper.Map<IEnumerable<CandleDto>>(response.Data ?? new List<MarketIndexDto>());

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private void SetItems()
        {
            var candles = _sources.GetTimeFrameCandles(_selectedTimeFrame);
            var containers = MACDService.Generate(candles);

            _items = (from candle in candles
                      from signal in containers.Where(f => f.Source.candle_date_time_utc == candle.candle_date_time_utc).DefaultIfEmpty()
                      select ((Func<CandleExtensionDto>)(() =>
                      {
                          var item = _mapper.Map<CandleExtensionDto>(candle);

                          item.signal = signal?.Histogram;

                          return item;
                      }))()).ToArray();

            StateHasChanged();
        }

        private void TimeFrameSelectionChanged(IEnumerable<TimeFrames> values)
        {
            SetItems();
        }


        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            if (_dateRangeTerm == value) { return; }

            _dateRangeTerm = value;

            await GetMarketIndicesAsync();

            SetItems();
        }
    }
}