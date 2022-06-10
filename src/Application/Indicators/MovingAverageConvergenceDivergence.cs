﻿using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Enums;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using TALib;

namespace Dreamrosia.Koin.Application.Indicators
{
    /// <summary>
    /// MACD 지표
    /// </summary>
    [DisplayName("MACD")]
    public class MovingAverageConvergenceDivergence : IIndicator<CandleDto, MovingAverageConvergenceDivergence.Container>
    {
        // TicTacTec.TA.Library.Core.MacdExt
        // 
        // Input Parameter
        //  - startIdx : start index for input data
        //  - endIdx : end index for input data
        //  - inReal : array of floats
        //  - optInFastPeriod
        //  - optInFastMAType
        //  - optInSlowPeriod
        //  - optInSlowMAType
        //  - optInSignalPeriod
        //  - optInSignalMAType
        // 
        // Return;
        //  - outBegIdx, 
        //  - outNBElement, 
        //  - outMACD 
        //  - outMACDSignal 
        //  - outMACDHist

        // MACD Line: (12-day EMA - 26-day EMA)
        // Signal Line: 9-day EMA of MACD Line
        // MACD Histogram: MACD Line - Signal Line

        [Display(Name = "지표이름")]
        public string Name { get; private set; }

        [Display(Name = "기준가격")]
        [DefaultValue(BasePrices.Close)]
        public BasePrices BasePrice { get; set; } = BasePrices.Close;

        [Display(Name = "단기")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [DefaultValue(5)]
        public int Short { get; set; } = 5;

        [Display(Name = "장기")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [DefaultValue(35)]
        public int Long { get; set; } = 35;

        [Display(Name = "이평기간")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [DefaultValue(9)]
        public int Signal { get; set; } = 9;

        [Display(Name = "이평방법")]
        [DefaultValue(Core.MAType.Ema)]
        public Core.MAType MAType { get; set; } = Core.MAType.Ema;

        private double[] OutMACD { get; set; }

        private double[] OutMACDHist { get; set; }

        private double[] OutSignal { get; set; }

        private int outBegIdx = -1;
        private int outNBElement = -1;

        protected Core.RetCode RetCode { get; set; }

        private readonly List<CandleDto> Sources = new List<CandleDto>();

        public List<Container> Containers { get; private set; } = new List<Container>();

        public MovingAverageConvergenceDivergence()
        {
            Name = "Moving Average Convergence Divergence";
        }

        public IEnumerable<Container> Generate(IEnumerable<CandleDto> source)
        {
            outBegIdx = -1;
            outNBElement = -1;

            OutMACD = null;
            OutSignal = null;
            OutMACDHist = null;

            Sources.Clear();
            Sources.AddRange(source);

            Containers.Clear();

            if (!Sources.Any()) { return Containers; }

            var dummy = Enumerable.Range(1, Long).Select(f => 0D).ToList();

            dummy.AddRange(Sources.Select(f => f.trade_price));

            double[] input = dummy.ToArray();

            OutMACD = new double[input.Length];
            OutSignal = new double[input.Length];
            OutMACDHist = new double[input.Length];

            RetCode = Core.MacdExt(input, 0, input.Length - 1,
                                   OutMACD, OutSignal, OutMACDHist,
                                   out outBegIdx, out outNBElement,
                                   MAType, MAType, MAType,
                                   Short, Long, Signal);

            //var size = dummy.Count() - Long;
            var skip = Sources.Count() - outNBElement;

            var macd = OutMACD[0..outNBElement];
            var signal = OutSignal[0..outNBElement];
            var histogram = OutMACDHist[0..outNBElement];

            List<Container> containers = new List<Container>();

            Container previous = null;
            Container current = null;

            for (int i = 0; i < outNBElement; i++)
            {
                current = new Container(Sources[i + skip], macd[i], signal[i], histogram[i]);

                if (i == 0)
                {
                    current.SeasonSignals = HistogramState(current);

                    previous = current;
                }
                else
                {
                    current.SeasonSignals = HistogramState(previous, current);

                    previous = current;
                }

                containers.Add(current);
            }

            Containers.AddRange(containers.Reverse<Container>());

            return Containers;
        }

        private SeasonSignals HistogramState(Container previous, Container current)
        {
            SeasonSignals state = SeasonSignals.Indeterminate;

            if (previous.Histogram > 0 && 0 < current.Histogram)
            {
                state = SeasonSignals.Above;
            }
            else if (previous.Histogram < 0 && 0 > current.Histogram)
            {
                state = SeasonSignals.Below;
            }
            else if (previous.Histogram < 0 && 0 < current.Histogram)
            {
                state = SeasonSignals.GoldenCross;
            }
            else if (previous.Histogram > 0 && 0 > current.Histogram)
            {
                state = SeasonSignals.DeadCross;
            }

            return state;
        }

        private SeasonSignals HistogramState(Container current)
        {
            SeasonSignals state = SeasonSignals.Indeterminate;

            if (0 < current.Histogram)
            {
                state = SeasonSignals.Above;
            }
            else if (0 > current.Histogram)
            {
                state = SeasonSignals.Below;
            }

            return state;
        }

        public SeasonSignals HistogramState(int index = 0)
        {
            return Containers.Count < index + 1 ? SeasonSignals.Indeterminate : Containers[index].SeasonSignals;
        }

        public class Container
        {
            public CandleDto Source { get; private set; }

            public double MACD { get; private set; }

            public double Signal { get; private set; }

            public double Histogram { get; private set; }

            public SeasonSignals SeasonSignals { get; set; } = SeasonSignals.Indeterminate;

            public Container(CandleDto source, double macd, double signal, double histogram)
            {
                Source = source;
                MACD = macd;
                Signal = signal;
                Histogram = histogram;
            }
        }
    }
}

