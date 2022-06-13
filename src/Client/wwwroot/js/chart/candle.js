var options = {
    series: [{
        name: "시세",
        data: seriesData
    }],
    chart: {
        type: 'candlestick',
        id: 'candles',
        height:'100%',
        toolbar: {
            show: true,
            autoSelected: 'pan',
        },
        zoom: {
            enabled: false
        },
        Animation: {
            enabled: false
        },
    },
    plotOptions: {
        candlestick: {
            colors: {
                upward: '#EF403C',
                downward: '#00B746'
            }
        }
    },
    xaxis: {
        type: 'category',
        labels: {
            formatter: func_chart_label.DateFormatter,
            rotate: 0,
            hideOverlappingLabels: true
        }
    },
    yaxis: {
        opposite: true,
        tickAmount: 10,
        crosshairs: {
            show: true,
        },
        labels: {
            formatter: func_chart_label.AxisRealNumberFormatter
        },
        tooltip: {
            enabled: true
        }
    },
    tooltip: {
        x: {
            formatter: func_chart_label.TooltipX
        },
        custom: func_chart_label.CandleTooltip
    }
};

var chart = new ApexCharts(document.querySelector("#chart-candlestick"), options);
chart.render();

var optionsSignal = {
    series: [{
        name: '신호',
        data: seriesDataLinear
    }],
    chart: {
        height: 170,
        type: 'line',
        brush: {
            enabled: true,
            target: 'candles'
        },
        selection: {
            enabled: true,
            fill: {
                color: '#ccc',
                opacity: 0.4
            },
            stroke: {
                color: '#0D47A1',
            }
        },
    },
    dataLabels: {
        enabled: false
    },
    plotOptions: {
        bar: {
            columnWidth: '80%',
            colors: {
                ranges: [{
                    from: -1000,
                    to: 0,
                    color: '#F15B46'
                }, {
                    from: 1,
                    to: 10000,
                    color: '#FEB019'
                }],

            },
        }
    },
    stroke: {
        width: 0
    },
    xaxis: {
        type: 'datetime',
        axisBorder: {
            offsetX: 13
        }
    },
    yaxis: {
        labels: {
            show: false
        }
    }
};

var chartSignal = new ApexCharts(document.querySelector("#chart-signal"), optionsSignal);
chartSignal.render();

var func_update_chart = {
    update_data: function (data) {
        chart.updateSeries(data);
    }
}