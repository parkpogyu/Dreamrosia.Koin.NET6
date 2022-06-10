using Dreamrosia.Koin.Bot.Constants;
using Dreamrosia.Koin.Bot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Dreamrosia.Koin.Bot.Extentions
{
    internal static class HostBuilderExtensions
    {
        internal static IHostBuilder UerSerilog(this IHostBuilder builder, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom
                                                  .Configuration(configuration)
                                                  .WriteTo.File($"Logs/{Environment.MachineName}/log_.txt", rollingInterval: RollingInterval.Day)
                                                  .CreateLogger();

            UPbit.Infrastructure.Logger.SetLogger(Log.Logger);

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }

        internal static IHostBuilder InitializeBot(this IHostBuilder builder, IConfiguration configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            ServerConstants.HubUrl = configuration[$"{environment}HubUrl"];

            Terminal.Ticket = Guid.NewGuid().ToString();
            Terminal.MachineName = Environment.MachineName;
            Terminal.Version = configuration["BotVersion"];
            Terminal.CurrentDirectory = Environment.CurrentDirectory;

            return builder;
        }
    }
}
