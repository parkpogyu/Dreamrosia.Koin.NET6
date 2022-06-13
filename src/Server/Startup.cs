using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.Enums;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Infrastructure.Extensions;
using Dreamrosia.Koin.Server.Extensions;
using Dreamrosia.Koin.Server.Managers.Preferences;
using Dreamrosia.Koin.Server.Middlewares;
using Dreamrosia.Koin.Shared.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Dreamrosia.Koin.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddSignalR();
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            services.AddCurrentUserService();
            services.AddSerialization();
            services.AddDatabase(_configuration);
            services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
            services.AddScoped<ServerPreferenceManager>();
            services.AddServerLocalization();
            services.AddIdentity();
            services.AddMultiAuthentication(_configuration);
            services.AddApplicationLayer();
            services.AddApplicationServices();
            services.AddUPbitServices();
            services.AddQuartz(services.GetServerConfiguration(_configuration));
            services.AddRepositories();
            services.AddSharedInfrastructure(_configuration);
            services.AddInfrastructureMappings();
            services.AddControllers().AddValidators();
            services.AddRazorPages();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            services.AddLazyCache();

            services.GetUPbitConfiguration(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizer<SharedLocalizerResources> localizer)
        {
            var section = _configuration.GetSection(nameof(ServerConfiguration));
            var serverConfig = section.Get<ServerConfiguration>();

            if (serverConfig.Mode != ServerModes.Test)
            {
                app.UseHttpsRedirection();
            }
            app.UseExceptionHandling(env);
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRequestLocalizationByCulture();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints();
            app.UseCors();
            app.Initialize(_configuration);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}