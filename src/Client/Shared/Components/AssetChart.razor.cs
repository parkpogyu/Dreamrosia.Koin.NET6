using ApexCharts;
using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class AssetChart : IAsyncDisposable
    {
        [CascadingParameter(Name = "Assets")]
        private IEnumerable<AssetExtensionDto> Assets
        {
            get => _assets;
            set
            {
                _assets = value;
            }
        }

        [Parameter] public bool IsReal { get; set; } = true;
        [Parameter] public DateTime? SignUpDate { get; set; }

        private bool _loaded { get; set; }
        private IEnumerable<AssetExtensionDto> _assets { get; set; }
        private IEnumerable<AssetExtensionDto> _items { get; set; } = new List<AssetExtensionDto>();
        private bool _fixedTooltip { get; set; } = true;
        private TimeFrames _selectedTimeFrame { get; set; } = TimeFrames.Week;
        private readonly string _divChartId = Guid.NewGuid().ToString();
        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivChartRendered { get; set; } = false;
        private string _divChartHeight { get; set; } = "100%";
        private string _assetChartHeight { get; set; } = "100%";
        private ApexChart<AssetExtensionDto> _refAssetChart { get; set; }
        private ApexChart<AssetExtensionDto> _refRangeChart { get; set; }
        private ApexChartOptions<AssetExtensionDto> _assetOptions;
        private ApexChartOptions<AssetExtensionDto> _rangeOptions;
        private string _selectedSeries { get; set; }
        private List<string> _selectedSerieses { get; set; }
        private readonly List<string> _seriseNames = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            await _jsRuntime.InvokeVoidAsync("LoadScript", "_content/Blazor-ApexCharts/js/apex-charts.min.js");
            await _jsRuntime.InvokeVoidAsync("LoadScript", "_content/Blazor-ApexCharts/js/blazor-apex-charts.js");
            await _jsRuntime.InvokeVoidAsync("LoadScript", "js/chart/chart-label.js");

            _mapper = new MapperConfiguration(c => { c.AddProfile<AssetProfile>(); }).CreateMapper();

            _selectedSerieses = new List<string>()
            {
                _localizer["Asset.InvsPnL"],
                _localizer["Asset.InvsAmt"],
            };

            _seriseNames.Add(_localizer["Asset.BalEvalAmt"]);
            _seriseNames.Add(_localizer["Asset.Deposit"]);
            _seriseNames.Add(_localizer["Asset.InvsAmt"]);
            _seriseNames.Add(_localizer["Asset.InvsPnL"]);
            _seriseNames.Add(_localizer["Asset.InvsPnLRat"]);
            _seriseNames.Add(_localizer["Asset.MDDInvsPnL"]);
            _seriseNames.Add(_localizer["Asset.MDDInvsPnLRat"]);
            if (IsReal)
            {
                _seriseNames.Add(_localizer["Asset.InAmt"]);
                _seriseNames.Add(_localizer["Asset.OutAmt"]);
            }
            else
            {
                _seriseNames.Add(_localizer["Asset.BorrowedAmt"]);
            }
            _seriseNames.Add(_localizer["Asset.BidAmt"]);
            _seriseNames.Add(_localizer["Asset.AskAmt"]);
            _seriseNames.Add(_localizer["Asset.PnL"]);
            _seriseNames.Add(_localizer["Market.Index.UBMI"]);

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

                    DrawChart(setData: true);
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
                _assetChartHeight = $"{(divHeight - 150)}px";
            }
            else
            {
                _divChartHeight = "auto";
                _assetChartHeight = $"{window.Height - 48}px";
            }
        }

        private void SetChartOptions()
        {
            #region Asset
            _assetOptions = new ApexChartOptions<AssetExtensionDto>();

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

            };

            _assetOptions.Tooltip.Fixed = _fixedTooltip ?
                                            new TooltipFixed()
                                            {
                                                Enabled = true,
                                                Position = "bottomLeft",
                                                OffsetX = 18,
                                                OffsetY = -18,
                                            } : null;

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
                SeriesName = _localizer["Asset.DssAmt"],
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

            SetYaxis();
            #endregion

            #region Range
            _rangeOptions = new ApexChartOptions<AssetExtensionDto>();

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

            _rangeOptions.Tooltip.Fixed = _fixedTooltip ?
                                            new TooltipFixed()
                                            {
                                                Enabled = true,
                                                Position = "bottomLeft",
                                                OffsetX = 18,
                                                OffsetY = -18,
                                            } : null;

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
                    Tooltip = new AxisTooltip()
                    {
                        Enabled = true,
                    }
                }
            };
            #endregion
        }

        private void SetYaxis()
        {
            var removes = (from axis in _assetOptions.Yaxis
                           from series in _selectedSerieses.Where(f => f.Equals(axis.SeriesName)).DefaultIfEmpty()
                           where series == null
                           select axis).ToArray();

            foreach (var remove in removes)
            {
                if (remove.SeriesName.Equals(_localizer["Asset.DssAmt"]) && remove.Show == null)
                {
                    continue;
                }

                _assetOptions.Yaxis.Remove(remove);
            }

            var appends = (from series in _selectedSerieses
                           from axis in _assetOptions.Yaxis.Where(f => f.SeriesName.Equals(series)).DefaultIfEmpty()
                           where axis == null
                           select series).ToArray();

            foreach (var name in appends)
            {
                if (name.Equals(_localizer["Market.Index.UBMI"]))
                {
                    _assetOptions.Yaxis.Add(new YAxis
                    {
                        SeriesName = name,
                        Opposite = false,
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
                }
                else
                {
                    _assetOptions.Yaxis.Add(new YAxis
                    {
                        SeriesName = _localizer["Asset.DssAmt"],
                        Show = false,
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
                }
            }
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

        public void DrawChart(bool setData = false)
        {
            try
            {
                if (setData)
                {
                    _items = _assets.GetTimeFrameAssets(_selectedTimeFrame);

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

        private void FixedTooltipChanged(bool value)
        {
            _fixedTooltip = value;

            _assetOptions.Tooltip.Fixed = _fixedTooltip ?
                                            new TooltipFixed()
                                            {
                                                Enabled = true,
                                                Position = "bottomLeft",
                                                OffsetX = 18,
                                                OffsetY = -18,
                                            } : null;

            _rangeOptions.Tooltip.Fixed = _fixedTooltip ?
                                            new TooltipFixed()
                                            {
                                                Enabled = true,
                                                Position = "bottomLeft",
                                                OffsetX = 18,
                                                OffsetY = -18,
                                            } : null;

            DrawChart(setData: false);
        }

        private void TimeFrameSelectionChanged(IEnumerable<TimeFrames> values)
        {
            DrawChart(setData: true);
        }

        private void SeriesSelectionChanged(IEnumerable<string> values)
        {
            _selectedSerieses = values.ToList();

            SetYaxis();

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

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}
