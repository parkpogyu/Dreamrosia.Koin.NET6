using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using TALib;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IMACDService : IIndicator<CandleDto, MacdContainer>
    {
        int Short { get; set; }

        int Long { get; set; }

        int Signal { get; set; }

        Core.MAType MAType { get; set; }

        SeasonSignals HistogramState(int index);
    }
}
