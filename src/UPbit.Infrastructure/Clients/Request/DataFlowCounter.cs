using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    internal class DataFlowCounter
    {
        public string Name { get; set; }

        public int Limited { get; private set; }

        public int WindowSpan { get; private set; }

        public int Buffer { get; private set; }

        private List<DateTime> SlidingWindow = new List<DateTime>();

        public DataFlowCounter(string name, int limited, int span, int buffer = 2)
        {
            Name = name;

            Limited = limited;
            WindowSpan = span;
            Buffer = buffer;
        }

        public void SetConditions(string name, int limited, int span, int buffer = 2)
        {
            Name = name;

            Limited = limited;
            WindowSpan = span;
            Buffer = buffer;
        }

        public void Increment()
        {
            DateTime now = DateTime.Now;

            var transfers = SlidingWindow.Where(f => f >= now.AddMilliseconds(-WindowSpan)).ToList();

            var count = transfers.Count();

            Logger.log.Debug("==>{0}, tranfers:{1:N0}, Span:{2:N0} Limited:{3:N0}", Name, count, WindowSpan, Limited);

            if (count == Limited - Buffer)
            {
                TimeSpan span = now.Subtract(transfers.Min());

                int padding = (int)(WindowSpan - span.TotalMilliseconds) + 1;

                int wait = padding;

                if (wait > 0)
                {
                    Task.Delay(wait).Wait();

                    Logger.log.Debug("==>{0} elapsed [{1,5:N0}ms], wait [{2,5:N0}ms]",
                                    Name,
                                    span.TotalMilliseconds,
                                    wait);
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
        public static int MinuteWindowSpan => 6000;
    }
}
