using System;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{

    public interface IBlazorTimerManager : IManager
    {
        void SetTimer(double interval, bool repeat) { }

        event Action OnElapsed;

    }
}
