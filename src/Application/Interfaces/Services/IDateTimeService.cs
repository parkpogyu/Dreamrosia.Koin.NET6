using System;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime Now { get; }
    }
}