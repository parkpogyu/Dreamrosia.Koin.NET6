using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Hubs;
using Dreamrosia.Koin.Server.Hubs;
using Dreamrosia.Koin.Server.Middlewares;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        internal static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseBrowserLink();
            }

            return app;
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
            => app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);
                endpoints.MapHub<SynchronizeHub>(ApplicationConstants.SynchronizeSignalR.HubUrl);
            });

        internal static IApplicationBuilder UseRequestLocalizationByCulture(this IApplicationBuilder app)
        {
            var supportedCultures = LocalizationConstants.SupportedLanguages.Select(l => new CultureInfo(l.Code)).ToArray();
            app.UseRequestLocalization(options =>
            {
                options.SupportedUICultures = supportedCultures;
                options.SupportedCultures = supportedCultures;
                options.DefaultRequestCulture = new RequestCulture(supportedCultures.First());
                options.ApplyCurrentCultureToResponseHeaders = true;
            });

            app.UseMiddleware<RequestCultureMiddleware>();

            return app;
        }

        internal static IApplicationBuilder Initialize(this IApplicationBuilder app, IConfiguration _configuration)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var provider = scope.ServiceProvider;

            var initializers = provider.GetRequiredService<IEnumerable<IDatabaseSeeder>>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            var tickerService = provider.GetRequiredService<IUPbitTickerService>();

            Task.Run(async () =>
            {
                await tickerService.InitializeAsync();
            });

            return app;
        }
    }
}