using Dreamrosia.Koin.Application.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Dreamrosia.Koin.Server.Extensions
{
    internal static class WebHostBuilderExtensions
    {
        internal static IWebHostBuilder UseServerConfiguration(this IWebHostBuilder webBuilder)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                          .AddJsonFile("appsettings.Development.json", true)
                                                          .AddEnvironmentVariables()
                                                          .Build();

            var section = configuration.GetSection(nameof(ServerConfiguration));

            var serverConfig = section.Get<ServerConfiguration>();

            if (serverConfig.Urls is null || serverConfig.Urls.Length == 0)
            {
                webBuilder.UseUrls(ServerConfiguration.DefaultUrl);
            }
            else
            {
                webBuilder.UseUrls(serverConfig.Urls);
            }

            //if (serverConfig.IsUseIIS)
            //{
            //    webBuilder.UseIISIntegration();
            //}

            return webBuilder;
        }
    }
}