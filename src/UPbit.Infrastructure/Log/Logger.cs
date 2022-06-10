using Serilog;

namespace Dreamrosia.Koin.UPbit.Infrastructure
{
    public class Logger
    {
        public static ILogger log { get; private set; }

        public static void SetLogger(ILogger logger)
        {
            log = logger;
        }
    }
}
