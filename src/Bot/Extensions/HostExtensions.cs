using Dreamrosia.Koin.Bot.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Extentions
{
    internal static class HostingAbstractionsHostExtensions
    {
        public static void RunMining(this IHost host)
        {
            Task.Run(async () =>
            {
                using var scope = host.Services.CreateScope();

                var provider = scope.ServiceProvider;

                var synchronize = provider.GetRequiredService<ISynchronizeService>();

                //var termService = provider.GetRequiredService<ITradingTermsService>();

                //await termService.GetTradingTermsAsync();
            });

            host.Run();
        }
    }
}
