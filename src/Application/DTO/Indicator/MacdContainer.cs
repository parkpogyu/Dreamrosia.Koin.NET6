using Dreamrosia.Koin.Shared.Enums;

namespace Dreamrosia.Koin.Application.DTO
{
    public class MacdContainer
    {
        public CandleDto Source { get; private set; }

        public double MACD { get; private set; }
        public double Signal { get; private set; }
        public double Histogram { get; private set; }
        public SeasonSignals SeasonSignals { get; set; } = SeasonSignals.Indeterminate;

        public MacdContainer(CandleDto source, double macd, double signal, double histogram)
        {
            Source = source;
            MACD = macd;
            Signal = signal;
            Histogram = histogram;
        }
    }
}
