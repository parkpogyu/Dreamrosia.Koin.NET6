using Dreamrosia.Koin.Bot.Extentions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Dreamrosia.Koin.Bot
{
    class Program
    {
        static IConfiguration configuration;

        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                      .AddJsonFile($"appsettings.{environment}.json", true)
                                                      .AddEnvironmentVariables()
                                                      .Build();

            CreateHostBuilder(args).Build()
                                   .RunMining();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .InitializeBot(configuration)
               .UerSerilog(configuration)
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddApplicationLayer();
                   services.AddQuartz();
                   services.AddServices();
               });
    }
}
