var func_chart_label = {

    AxisRealNumberFormatter: function (value) {

        var abs = Math.abs(value);

        if (0 < abs && abs < 1) {
            return new Intl.NumberFormat('ko-KR', { minimumFractionDigits: 4, maximumFractionDigits: 4 }).format(value);
        }
        else if (1 < abs && abs < 100) {
            return new Intl.NumberFormat('ko-KR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value);
        }
        else {
            return new Intl.NumberFormat('ko-KR', { maximumFractionDigits: 0}).format(value);
        }
    },

    DateFormatter: function (value) {

        if (value == "undefined") {
            return '';
        }

        let date = new Date(value);

        let month = date.getMonth() + 1;
        let day = date.getDate();

        month = month >= 10 ? month : '0' + month;
        day = day >= 10 ? day : '0' + day;

        let convert = date.getFullYear() + '-' + month + '-' + day;

        return convert;
    },

    TooltipX: function (value, opts) {

        if (value === undefined) { return ''; }

        return opts.w.globals.categoryLabels[value - 1];
    },

    CandleTooltip: function ({ seriesIndex, dataPointIndex, w }) {

        if (w.globals.seriesCandleO[seriesIndex].length < dataPointIndex - 1) {
            return "";
        }

        const o = w.globals.seriesCandleO[seriesIndex][dataPointIndex]
        const h = w.globals.seriesCandleH[seriesIndex][dataPointIndex]
        const l = w.globals.seriesCandleL[seriesIndex][dataPointIndex]
        const c = w.globals.seriesCandleC[seriesIndex][dataPointIndex]

        return (
            '<div class="apexcharts-tooltip-box apexcharts-tooltip-candlestick">' +
            '<div>O: <span class="value">' + func_chart_label.AxisRealNumberFormatter(o) + '</span></div>' +
            '<div>H: <span class="value">' + func_chart_label.AxisRealNumberFormatter(h) + '</span></div>' +
            '<div>L: <span class="value">' + func_chart_label.AxisRealNumberFormatter(l) + '</span></div>' +
            '<div>C: <span class="value">' + func_chart_label.AxisRealNumberFormatter(c) + '</span></div>' +
            '</div>'
        )
    }
}