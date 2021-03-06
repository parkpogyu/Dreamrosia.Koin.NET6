using ApexCharts;
using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class CandleChart : IAsyncDisposable
    {
        [CascadingParameter(Name = "Candles")]
        private IEnumerable<CandleExtensionDto> Candles
        {
            get => _items;
            set
            {
                _items = value;

                if (_isDivChartRendered)
                {
                    DrawChart();
                }
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        private bool _loaded;
        private IEnumerable<CandleExtensionDto> _items { get; set; }

        private Guid _resizeSubscribedId { get; set; }
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
            await _jsRuntime.InvokeVoidAsync("LoadScript", "_content/Blazor-ApexCharts/js/apex-charts.min.js");
            await _jsRuntime.InvokeVoidAsync("LoadScript", "_content/Blazor-ApexCharts/js/blazor-apex-charts.js");
            await _jsRuntime.InvokeVoidAsync("LoadScript", "js/chart/chart-label.js");

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
                    _resizeSubscribedId = await _resizeService.Subscribe((size) =>
                    {
                        if (!_isDivChartRendered) { return; }

                        InvokeAsync(SetDivHeightAsync);

                    }, new ResizeOptions
                    {
                        NotifyOnBreakpointOnly = false,
                    });
                }
                else
                {
                    if (_isDivChartRendered) { return; }

                    var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divChartId);

                    if (!isRendered) { return; }

                    _isDivChartRendered = isRendered;

                    await SetDivHeightAsync();

                    DrawChart();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                await base.OnAfterRenderAsync(firstRender);
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

            //_candleOptions.Stroke = new Stroke()
            //{
            //    Curve = Curve.Smooth,
            //};

            //_candleOptions.Legend = new Legend()
            //{
            //    Show = false,
            //};

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

            _candleOptions.Yaxis = new List<YAxis>()
            {
                new YAxis
                {
                    SeriesName = "Candle",
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
                },

                //new YAxis
                //{
                //    SeriesName = "Index",
                //    Title =  new AxisTitle()
                //    {
                //        Text ="Index"
                //    },
                //    Opposite = false,
                //    Crosshairs = new AxisCrosshairs()
                //    {
                //        Show = true,
                //    },
                //    Labels = new YAxisLabels
                //    {
                //        Formatter = @"func_chart_label.AxisRealNumberFormatter",
                //    },
                //    Tooltip = new AxisTooltip() 
                //    {
                //        Enabled = true,
                //    },
                //},
            };
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

            _rangeOptions.Yaxis = new List<YAxis>()
            {
                new YAxis
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
                }
            };
            #endregion
        }

        public void DrawChart()
        {
            try
            {
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

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}
