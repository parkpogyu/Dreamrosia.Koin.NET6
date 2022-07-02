using ApexCharts;
using AutoMapper;
using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Mappings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class CandleChart : IDisposable
    {
        [Inject] private IMACDService MACDService { get; set; }

        [CascadingParameter(Name = "Candles")]
        private IEnumerable<CandleDto> Candles
        {
            get => _items;
            set
            {
                _items = value;

                if (_isDivChartRendered)
                {
                    DrawChart(setData: true);
                }
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        private bool _loaded;
        private IEnumerable<CandleDto> _items { get; set; }
        private IEnumerable<CandleExtensionDto> _candles { get; set; } = new List<CandleExtensionDto>();
        private bool _isDivChartRendered { get; set; } = false;
        private string _divChartHeight { get; set; } = "100%";
        private string _candleChartHeight { get; set; } = "100%";
        private readonly string _divChartId = Guid.NewGuid().ToString();
        private ApexChart<CandleExtensionDto> _refCandleChart { get; set; }
        private ApexChart<CandleExtensionDto> _refRangeChart { get; set; }
        private ApexChartOptions<CandleExtensionDto> _candleOptions;
        private ApexChartOptions<CandleExtensionDto> _rangeOptions;

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("loadScript", "_content/Blazor-ApexCharts/js/apex-charts.min.js");
            await _jsRuntime.InvokeVoidAsync("loadScript", "_content/Blazor-ApexCharts/js/blazor-apex-charts.js");
            await _jsRuntime.InvokeVoidAsync("loadScript", "js/chart/chart-label.js");

            _mapper = new MapperConfiguration(c => { c.AddProfile<CandleProfile>(); }).CreateMapper();

            SetChartOptions();

            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeListener.OnResized += OnWindowResized;
                }

                if (_isDivChartRendered) { return; }

                var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divChartId);

                if (!isRendered) { return; }

                _isDivChartRendered = isRendered;

                await SetDivHeightAsync();

                DrawChart(setData: true);
            }
            catch (Exception)
            {
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divChartId);

            if (rect is null) { return; }

            if (BoundingClientRect.IsMatchMediumBreakPoints(window.Height))
            {
                var divHeight = (window.Height - rect.Top - 8);

                _divChartHeight = $"{divHeight}px";
                _candleChartHeight = $"{(divHeight - 150)}px";
            }
            else
            {
                _divChartHeight = "auto";
                _candleChartHeight = $"{window.Height - 48}px";
            }
        }

        private void SetChartOptions()
        {
            #region Candle
            _candleOptions = new ApexChartOptions<CandleExtensionDto>();

            _candleOptions.Chart = new Chart()
            {
                Id = "Candle",

                Height = "600px",

                Animations = new Animations()
                {
                    Enabled = false,
                },

                Locales = new List<ChartLocale>()
                {
                    ApexchartsLocales.Ko,
                },

                DefaultLocale = "ko",

                Toolbar = new Toolbar()
                {
                    Tools = new Tools()
                    {
                        Download = false,
                        Pan = false,
                        Reset = false,
                        Zoom = false,
                    }
                },

            };

            _candleOptions.PlotOptions = new PlotOptions()
            {
                Candlestick = new PlotOptionsCandlestick()
                {
                    Colors = new PlotOptionsCandlestickColors()
                    {
                        Downward = "#00B746",
                        Upward = "#EF403C",
                    },
                }
            };

            _candleOptions.Tooltip = new Tooltip
            {
                X = new TooltipX
                {
                    Formatter = @"func_chart_label.TooltipX",
                },

                Custom = @"func_chart_label.CandleTooltip",
            };

            _candleOptions.Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Formatter = @"func_chart_label.DateFormatter",

                    Rotate = 0,
                    HideOverlappingLabels = true,
                }
            };

            _candleOptions.Yaxis = new List<YAxis>();
            _candleOptions.Yaxis.Add(new YAxis
            {
                Opposite = true,
                Crosshairs = new AxisCrosshairs()
                {
                    Show = true,
                },
                Labels = new YAxisLabels
                {
                    Formatter = @"func_chart_label.AxisRealNumberFormatter",
                },
                Tooltip = new AxisTooltip()
                {
                    Enabled = true,
                },
            });
            #endregion

            #region Signal
            _rangeOptions = new ApexChartOptions<CandleExtensionDto>();
            _rangeOptions.Annotations = new Annotations()
            {
                Position = "front",
                Yaxis = new List<AnnotationsYAxis>()
                {
                    new AnnotationsYAxis()
                    {
                        Y = 0,
                        BorderColor= "#ff1c69",
                        FillColor= "#ff1c69",
                        OffsetX= -0.1,
                    },

                    new AnnotationsYAxis()
                    {
                        Y = 0,
                        BorderColor= "#ff1c69",
                        FillColor= "#ff1c69",
                        OffsetX= 0.1,
                    },
                }
            };

            _rangeOptions.Chart = new Chart()
            {
                Animations = new Animations()
                {
                    Enabled = false,
                },

                Locales = new List<ChartLocale>()
                {
                    ApexchartsLocales.Ko,
                },

                DefaultLocale = "ko",

                Brush = new Brush()
                {
                    Target = "Candle",
                    Enabled = true,
                },

                Selection = new Selection()
                {
                    Enabled = true,
                }
            };

            _rangeOptions.Stroke = new Stroke()
            {
                Curve = Curve.Smooth,
            };

            _rangeOptions.Tooltip = new Tooltip
            {
                X = new TooltipX
                {
                    Formatter = @"func_chart_label.TooltipX",
                    Show = true,
                },
            };

            _rangeOptions.Xaxis = new XAxis
            {
                AxisTicks = new AxisTicks()
                {
                    Show = false,
                },
                Labels = new XAxisLabels
                {
                    Formatter = @"func_chart_label.DateFormatter",
                    Show = false,
                },
            };

            _rangeOptions.Yaxis = new List<YAxis>();
            _rangeOptions.Yaxis.Add(new YAxis
            {
                Opposite = true,
                TickAmount = 2,
                Crosshairs = new AxisCrosshairs()
                {
                    Show = true,
                },
                Labels = new YAxisLabels
                {
                    Formatter = @"func_chart_label.AxisRealNumberFormatter",
                },
                Tooltip = new AxisTooltip()
                {
                    Enabled = true,
                },
            });
            #endregion
        }

        private void SetChartData()
        {
            var containers = MACDService.Generate(_items);

            _candles = (from lt in _items
                        from rt in containers.Where(f => f.Source.candle_date_time_utc == lt.candle_date_time_utc).DefaultIfEmpty()
                        select ((Func<CandleExtensionDto>)(() =>
                        {
                            var item = _mapper.Map<CandleExtensionDto>(lt);

                            item.signal = Convert.ToDouble(rt?.Histogram);

                            return item;
                        }))()).ToArray();
        }

        public void DrawChart(bool setData)
        {
            try
            {
                if (setData)
                {
                    SetChartData();
                }

                StateHasChanged();

                Task.Run(async () =>
                {
                    await _refCandleChart.RenderAsync();
                    await _refRangeChart.RenderAsync();
                });

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
            }
        }

        private void OnWindowResized(object sender, BrowserWindowSize e)
        {
            Task.Run(async () =>
            {
                if (_isDivChartRendered)
                {
                    await SetDivHeightAsync();

                    DrawChart(setData: false);
                }
            });
        }

        public void Dispose()
        {
            _resizeListener.OnResized -= OnWindowResized;
        }
    }
}
