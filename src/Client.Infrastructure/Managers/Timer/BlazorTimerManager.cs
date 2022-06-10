using System;
using System.Timers;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    // https://wellsb.com/csharp/aspnet/blazor-timer-navigate-programmatically/

    public sealed class BlazorTimerManager : IBlazorTimerManager
    {
        private Timer _timer;

        public void SetTimer(double interval, bool repeat)
        {
            _timer = new Timer(interval);
            _timer.Elapsed += NotifyTimerElapsed;
            _timer.AutoReset = repeat;
            _timer.Enabled = true;
        }

        public event Action OnElapsed;

        private void NotifyTimerElapsed(object source, ElapsedEventArgs e)
        {
            OnElapsed?.Invoke();

            if (!_timer.AutoReset)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
