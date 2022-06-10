using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Dreamrosia.Koin.Server.Extensions
{
    internal static class HostBuilderExtensions
    {
        internal static IHostBuilder UseSerilog(this IHostBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                          .AddJsonFile($"appsettings.{environment}.json", true)
                                                          .AddEnvironmentVariables()
                                                          .Build();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            UPbit.Infrastructure.Logger.SetLogger(Log.Logger);

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }
    }
}