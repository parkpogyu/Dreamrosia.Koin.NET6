﻿using ApexCharts;
using Newtonsoft.Json;

namespace Dreamrosia.Koin.Client.Shared
{
    public static class ApexchartsLocales
    {
        public static ChartLocale Ko => JsonConvert.DeserializeObject<ChartLocale>(json);

        private static string json => @"
        {
            'name': 'ko',
            'options': {
                'months': [
                    '1월',
                    '2월',
                    '3월',
                    '4월',
                    '5월',
                    '6월',
                    '7월',
                    '8월',
                    '9월',
                    '10월',
                    '11월',
                    '12월'
                ],
                'shortMonths': [
                    '1월',
                    '2월',
                    '3월',
                    '4월',
                    '5월',
                    '6월',
                    '7월',
                    '8월',
                    '9월',
                    '10월',
                    '11월',
                    '12월'
                ],
                'days': [
                    '일요일',
                    '월요일',
                    '화요일',
                    '수요일',
                    '목요일',
                    '금요일',
                    '토요일'
                ],
                'shortDays': [ '일', '월', '화', '수', '목', '금', '토' ],
                'toolbar': {
                    'exportToSVG': 'SVG 다운로드',
                    'exportToPNG': 'PNG 다운로드',
                    'exportToCSV': 'CSV 다운로드',
                    'menu': '메뉴',
                    'selection': '선택',
                    'selectionZoom': '선택영역 확대',
                    'zoomIn': '확대',
                    'zoomOut': '축소',
                    'pan': '패닝',
                    'reset': '원래대로'
                }
            }
        }";
    }
}
