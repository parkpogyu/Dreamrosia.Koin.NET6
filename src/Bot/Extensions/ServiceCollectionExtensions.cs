using Dreamrosia.Koin.Bot.Interfaces;
using Dreamrosia.Koin.Bot.Schedules;
using Dreamrosia.Koin.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Reflection;

namespace Dreamrosia.Koin.Bot.Extentions
{
    internal static class ServiceCollectionExtensions
    {

        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        internal static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ISynchronizeService, SynchronizeService>();
            services.AddSingleton<ITradeOrderService, TradeOrderService>();
            services.AddSingleton<IUPbitService, UPbitService>();

            return services;
        }

        internal static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.AddJobAndTrigger<SynchronizeJob>();
                q.AddJobAndTrigger<TransactionJob>();
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return services;
        }

        internal static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz) where T : IJob
        {
            // Use the name of the IJob as the appsettings.json key
            string jobName = typeof(T).Name;

            // Try and load the schedule from configuration
            var configKey = $"Quartz:{jobName}";
            var cronSchedule = "* * * * * ?";

            if (typeof(T) == typeof(SynchronizeJob))
            {
                var random = new Random();

                var seed = random.Next(0, 60);

                cronSchedule = string.Format("{0},{1} * * * * ?", seed, (seed + 30) % 60);
            }

            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

            quartz.AddTrigger(opts => opts
                  .ForJob(jobKey)
                  .WithIdentity(jobName + "-trigger")
                  .WithCronSchedule(cronSchedule)); // use the schedule from configuration
        }
    }
}
