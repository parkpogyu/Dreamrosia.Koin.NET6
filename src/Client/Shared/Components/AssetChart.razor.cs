using ApexCharts;
using BlazorPro.BlazorSize;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class AssetChart : IDisposable
    {

        [CascadingParameter(Name = "Assets")]
        private IEnumerable<AssetDto> Assets
        {
            get => _assets;
            set
            {
                _assets = value;
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;

        [Parameter] public DateTime? SignUpDate { get; set; }

        private IEnumerable<AssetDto> _assets { get; set; }
        private IEnumerable<AssetDto> _items { get; set; } = new List<AssetDto>();

        private TimeFrames _selectedTimeFrame { get; set; } = TimeFrames.Week;

        private readonly string _divChartId = Guid.NewGuid().ToString();

        private bool _isDivChartRendered { get; set; } = false;
        private string _divChartHeight { get; set; } = "100%";
        private string _assetChartHeight { get; set; } = "85%";
        private ApexChart<AssetDto> _refAssetChart { get; set; }
        private ApexChart<AssetDto> _refRangeChart { get; set; }
        private ApexChartOptions<AssetDto> _assetOptions;
        private ApexChartOptions<AssetDto> _rangeOptions;

        private string _selectedSeries { get; set; }
        private List<string> _selectedSerieses { get; set; }

        protected override void OnInitialized()
        {
            _selectedSerieses = new List<string>()
            {
                _localizer["Asset.DssAmt"],
                _localizer["Asset.InvsPnL"],
                _localizer["Asset.InvsAmt"],
            };

            SetChartOptions();
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

            var divHeight = (window.Height - rect.Top - 8);

            _divChartHeight = $"{divHeight}px";
            _assetChartHeight = $"{(divHeight - 150)}px";
        }

        private void SetChartOptions()
        {
            #region Asset
            _assetOptions = new ApexChartOptions<AssetDto>();

            _assetOptions.Chart = new Chart()
            {
                Id = "Asset",

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

            _assetOptions.Legend = new Legend()
            {
                Position = LegendPosition.Top,

                OnItemClick = new LegendOnItemClick()
                {
                    ToggleDataSeries = true,
                },

                OnItemHover = new LegendOnItemHover()
                {
                    HighlightDataSeries = true,
                }
            };

            _assetOptions.Stroke = new Stroke()
            {
                Curve = Curve.Smooth,
            };

            _assetOptions.Tooltip = new Tooltip
            {
                X = new TooltipX
                {
                    Formatter = @"func_chart_label.TooltipX",
                },

                Custom = @"func_chart_label.AssetTooltip",
            };

            if (SignUpDate is not null)
            {
                _assetOptions.Annotations = new Annotations
                {
                    Xaxis = new List<AnnotationsXAxis>
                    {
                        new AnnotationsXAxis
                        {
                           Label = new Label
                           {
                                Text = $"{_localizer["User.SignupDate"]}:{SignUpDate:d}",
                                Style = new Style{ Background="red" },
                                Orientation = "horizontal",
                           },

                           X = SignUpDate?.Date,
                           BorderWidth = 3,
                           StrokeDashArray = 2,
                           BorderColor = "red",
                        } ,
                    },
                };
            }

            _assetOptions.Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Formatter = @"func_chart_label.DateFormatter",

                    Rotate = 0,
                    HideOverlappingLabels = true,
                }
            };

            _assetOptions.Yaxis = new List<YAxis>();
            _assetOptions.Yaxis.Add(new YAxis
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

            //_assetOptions.Yaxis.Add(new YAxis
            //{
            //    SeriesName = _localizer["Asset.PnLRat"],

            //    Crosshairs = new AxisCrosshairs()
            //    { 
            //        Show = true,
            //    },
            //    Labels = new AxisLabels
            //    {
            //        Formatter = @"func_chart_label.AxisRealNumberFormatter",
            //    },
            //    Tooltip = new AxisTooltip()
            //    { 
            //       Enabled = true,
            //    }

            //});
            #endregion

            #region Range
            _rangeOptions = new ApexChartOptions<AssetDto>();

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
                    Target = "Asset",
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
                }
            });
            #endregion
        }

        private void SetAnnotationOptions()
        {
            if (SignUpDate is null || _assetOptions is null || !_items.Any()) { return; }

            var approximate = _items.Where(f => f.created_at >= SignUpDate).OrderBy(f => f.created_at).FirstOrDefault();

            if (approximate is null) { return; }

            _assetOptions.Annotations = new Annotations
            {
                Xaxis = new List<AnnotationsXAxis>
                {
                    new AnnotationsXAxis
                    {
                       Label = new Label
                       {
                            Text = $"{_localizer["User.SignupDate"]}:{SignUpDate:d}",
                            Orientation = "horizontal",

                            Style = new Style()
                            {
                                 Padding = new Padding( )
                                 {
                                      Left = 2,
                                      Top = 2,
                                      Right = 2,
                                      Bottom = 2,
                                 },
                            }
                       },

                       X = $"{approximate.created_at:d}",
                       BorderWidth = 3,
                       StrokeDashArray = 0,
                    } ,
                },
            };
        }

        private void SetChartData()
        {
            if (!Assets.Any())
            {
                _items = Assets.ToArray();

                return;
            }

            DateTime first_day_of_period = Assets.Min(f => f.created_at);

            List<List<AssetDto>> frameAssets = new List<List<AssetDto>>();
            List<AssetDto> assets = new List<AssetDto>();

            frameAssets.Add(assets);

            if (_selectedTimeFrame == TimeFrames.Day)
            {
                _items = Assets.ToArray();

                return;
            }
            else if (_selectedTimeFrame == TimeFrames.Week)
            {
                foreach (var asset in Assets)
                {
                    if (asset.created_at.DayOfWeek == DayOfWeek.Monday)
                    {
                        assets = new List<AssetDto>();

                        frameAssets.Add(assets);
                    }

                    assets.Add(asset);
                }
            }
            else if (_selectedTimeFrame == TimeFrames.Month)
            {
                foreach (var asset in Assets)
                {
                    if (asset.created_at.Day == 1)
                    {
                        assets = new List<AssetDto>();

                        frameAssets.Add(assets);
                    }

                    assets.Add(asset);
                }
            }
            else if (_selectedTimeFrame == TimeFrames.Year)
            {
                var year = first_day_of_period.Year;

                foreach (var asset in Assets)
                {
                    if (asset.created_at.Year != year)
                    {
                        first_day_of_period = asset.created_at;

                        year = first_day_of_period.Year;

                        assets = new List<AssetDto>();

                        frameAssets.Add(assets);
                    }

                    assets.Add(asset);
                }
            }

            _items = frameAssets.Where(f => f.Any())
                                .Select(f =>
                                {
                                    var head = f.First();
                                    var rear = f.Last();

                                    AssetDto group = new AssetDto()
                                    {
                                        BalEvalAmt = rear.BalEvalAmt,
                                        Deposit = rear.Deposit,
                                        InvsAmt = rear.InvsAmt,
                                        BorrowedAmt = rear.BorrowedAmt,
                                        InAmt = f.Sum(f => f.InAmt),
                                        OutAmt = f.Sum(f => f.OutAmt),
                                        AskAmt = f.Sum(f => f.AskAmt),
                                        BidAmt = f.Sum(f => f.BidAmt),
                                        Fee = f.Sum(f => f.Fee),
                                        PnL = f.Sum(f => f.PnL),
                                        MaxDssAmt = f.Max(f => f.MaxDssAmt),
                                        MaxInvsPnL = f.Max(f => f.MaxInvsPnL),
                                        PositionCount = f.Sum(f => f.PositionCount),
                                        created_at = rear.created_at,
                                    };

                                    return group;
                                }).ToArray();
        }

        public void DrawChart(bool setData = false)
        {
            try
            {
                if (setData)
                {
                    SetChartData();
                    SetAnnotationOptions();
                }

                StateHasChanged();

                Task.Run(async () =>
                {
                    await _refAssetChart.RenderAsync();
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

        private void TimeFrameSelectionChanged(IEnumerable<TimeFrames> values)
        {
            DrawChart(setData: true);
        }

        private void SeriesSelectionChanged(IEnumerable<string> values)
        {
            _selectedSerieses = values.ToList();

            DrawChart(setData: false);
        }

        private string GetSeriesSelectionText(List<string> values)
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
