using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    internal class DataFlowCounter
    {
        public string Name { get; set; }
        public int Limited { get; private set; }
        public int WindowSpan { get; private set; }
        public int Padding { get; private set; }
        public CounterModes CounterMode { get; private set; }
        private Stopwatch Stopwatch { get; set; }
        private int Count { get; set; }
        private int Delay => 100;
        private List<DateTime> SlidingWindow { get; set; }

        public DataFlowCounter(string name, int limited, int span, CounterModes mode = CounterModes.Stopwatch, int padding = 2)
        {
            Name = name;

            Limited = limited;
            WindowSpan = span;
            Padding = padding;
            CounterMode = mode;

            if (mode == CounterModes.Stopwatch)
            {
                Stopwatch = new Stopwatch();
            }
            else
            {
                SlidingWindow = new List<DateTime>();
            }
        }

        public void Increment()
        {
            if (CounterMode == CounterModes.Stopwatch)
            {
                if (Count == 0) { Stopwatch.Start(); }

                StopwatchIncrement();
            }
            else
            {
                SlidingWindowIncrement();
            }
        }

        private void StopwatchIncrement()
        {
            if (Stopwatch.ElapsedMilliseconds > WindowSpan)
            {
                Logger.log.Debug($"==> OverTime {Name}:[{Limited:N0}/{WindowSpan / 1000:N0}s], Transfers:{Count:N0}, Elapsed:{Stopwatch.Elapsed:c}");

                Count = 1;
                Stopwatch.Restart();
            }
            else
            {
                Count++;

                if (Limited <= Count)
                {
                    int delay = (int)(WindowSpan - Stopwatch.ElapsedMilliseconds) + Delay;

                    Logger.log.Debug($"==> OverCount {Name}[{Limited:N0}/{WindowSpan / 1000:N0}s], Transfers:{Count - 1:N0},  Elapsed:{Stopwatch.Elapsed:c}, Delay:{delay:N0}ms");

                    Task.Delay(delay).Wait();

                    Count = 1;
                    Stopwatch.Restart();
                }
            }
        }

        private void SlidingWindowIncrement()
        {
            DateTime now = DateTime.Now;

            var transfers = SlidingWindow.Where(f => f >= now.AddMilliseconds(-WindowSpan)).ToList();

            var count = transfers.Count();

            //Logger.log.Debug("==> {0}, tranfers:{1:N0}, Span:{2:N0} Limited:{3:N0}", Name, count, WindowSpan, Limited);

            if (Limited <= count + Padding)
            {
                TimeSpan span = now.Subtract(transfers.Min());

                int delay = (int)(WindowSpan - span.TotalMilliseconds);

                if (delay > 0)
                {
                    delay = delay + Delay;
                    Task.Delay(delay).Wait();
                    Logger.log.Debug($"==> {Name}[{Limited:N0}/{WindowSpan / 1000:N0}s], Transfers:{count:N0}, Elapsed:{span.Milliseconds:N0}ms, Delay:{delay:N0}ms");
                }
            }

            var redundants = SlidingWindow.Where(f => f < now.AddMilliseconds(-WindowSpan)).ToList();

            if (redundants.Any())
            {
                foreach (var redundant in redundants)
                {
                    SlidingWindow.Remove(redundant);
                }
            }

            SlidingWindow.Add(DateTime.Now);
        }

        public static int SecondWindowSpan => 1000;
        public static int MinuteWindowSpan => 60000;

        public enum CounterModes
        {
            Stopwatch,
            SlingWindow,
        }
    }
}
