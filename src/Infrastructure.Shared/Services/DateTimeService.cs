using Dreamrosia.Koin.Application.Interfaces.Services;
using System;

namespace Dreamrosia.Koin.Infrastructure.Shared.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}