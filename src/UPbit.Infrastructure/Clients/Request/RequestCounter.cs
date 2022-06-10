using System.Collections.Generic;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Clients
{
    internal class RequestCounter
    {
        private DataFlowCounter SecondCounter;

        private DataFlowCounter MinuteCounter;

        public RequestCounter(string name, int second, int minute)
        {
            SecondCounter = new DataFlowCounter(string.Format("Sec-{0}", name), second, DataFlowCounter.SecondWindowSpan);
            MinuteCounter = new DataFlowCounter(string.Format("Min-{0}", name), minute, DataFlowCounter.MinuteWindowSpan);
        }

        public void Increment()
        {
            SecondCounter.Increment();
            MinuteCounter.Increment();
        }

        private static RequestCounter GetCounter(IUPbitWebApiClient client)
        {
            RequestCounter counter;

            lock (locker)
            {
                string key = string.Empty;

                if (client is IQuotaionClient)
                {
                    key = typeof(IQuotaionClient).Name;
                }
                else
                {
                    if (client is ExOrderPost)
                    {
                        key = typeof(ExOrderPost).Name;
                    }
                    else
                    {
                        key = typeof(IExchangeClient).Name;
                    }
                }

                if (RequestCounters.ContainsKey(key))
                {
                    counter = RequestCounters[key];
                }
                else
                {
                    counter = new RequestCounter(key, client.PerSecondLimted, client.PerMinuteLimted);

                    RequestCounters.Add(key, counter);

                    //Logger.log.Information("==> Request Counter: {0:D2}:{1}: {2:N0}-{3:N0} ",
                    //                            RequestCounters.Count,
                    //                            type.Name,
                    //                            client.PerSecondLimted,
                    //                            client.PerMinuteLimted);
                }

                return counter;
            }
        }

        public static void IncrementCounter(IUPbitWebApiClient client)
        {
            RequestCounter counter = GetCounter(client);

            lock (locker)
            {
                counter.Increment();
            }
        }

        private static readonly Dictionary<string, RequestCounter> RequestCounters = new Dictionary<string, RequestCounter>();

        private static readonly object locker = new object();
    }
}
