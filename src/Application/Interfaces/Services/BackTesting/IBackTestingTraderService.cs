using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IBackTestingTraderService
    {

        DateTime GetFirstSignalDate(TimeFrames frame);
        PaperPositionDto Position { get; }
        List<PaperPositionDto> Positions { get; }
        List<PaperOrderDto> Orders { get; }
        Task Prepare(BackTestingRequestDto model, string market, DateTime head, DateTime rear);
        void Simulate(DateTime date, TimeFrames frame);
    }
}