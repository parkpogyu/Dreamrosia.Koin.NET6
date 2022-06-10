var delay = 300;
var timer = null;

window.addEventListener('load', function () {
    clearTimeout(timer);
    timer = setTimeout(function () {
        if (l_data != null) {
            drawChart(l_data);
        }
    }, delay);
});

window.addEventListener('resize', function () {
    if (this.resizeTO) clearTimeout(this.resizeTO);
    this.resizeTO = setTimeout(function () {
        window.dispatchEvent(new Event('resizeEnd'));
    }, delay);
});

window.addEventListener('resizeEnd', function () {
    if (l_data != null) {
        drawChart(l_data);
    }
});

var l_data;

function drawChart(data) {

    l_data = data;

    var dt = new google.visualization.DataTable();
    dt.addColumn('date', '일자');
    dt.addColumn('number', '평가금액');
    dt.addColumn('number', '보유현금');
    dt.addColumn('number', '보유자산');

    l_data.forEach(function (item, index, arr2) {
        dt.addRow([new Date(item[0]), item[1], item[2], item[3]]);
    });

    //dt.addRows([
    //    ...l_data
    //]);

    var chart_div = document.getElementById("chart_div");

    var rect = chart_div.getBoundingClientRect();

    var chart_height = window.innerHeight - rect.top - 7;

    // Set chart options
    var options = {
        legend: { position: 'top' },
        curveType: 'function',
        height: chart_height,
        crosshair: { trigger: 'both' }, // Display crosshairs on focus and selection.
        explorer: {
            actions: ['dragToZoom', 'rightClickToReset'],
            axis: 'horizontal',
            keepInBounds: true,
            maxZoomIn: 4.0
        },
        series: {
            0: {
                targetAxisIndex: '1'
            },
        },
        vAxis: {
            textPosition: 'in',
            gridlines: {
                count: 10
            },
            minorGridlines: {
                count:0
            }
        },
        hAxis: {
            textPosition: 'in',
            format:'yyyy-MM-dd',
            slantedText: 'false',
            maxTextLines:'1',
            maxAlternation:'1',
            allowContainerBoundaryTextCutoff:'true'
        },
        chartArea: {
            with: '100%',
            height: '100%',
            left: '15',
            right: '15',
            top: '35',
            bottom: '15'
        },
    }
    var chart = new google.visualization.LineChart(document.getElementById('chart_div'));

    chart.draw(dt, options);
}

export function setOnLoadCallback(data) {

    google.charts.setOnLoadCallback(drawChart(data))
}