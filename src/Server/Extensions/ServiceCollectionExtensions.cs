using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.Enums;
using Dreamrosia.Koin.Application.Interfaces.Serialization.Options;
using Dreamrosia.Koin.Application.Interfaces.Serialization.Serializers;
using Dreamrosia.Koin.Application.Interfaces.Serialization.Settings;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Serialization.JsonConverters;
using Dreamrosia.Koin.Application.Serialization.Options;
using Dreamrosia.Koin.Application.Serialization.Serializers;
using Dreamrosia.Koin.Application.Serialization.Settings;
using Dreamrosia.Koin.Infrastructure;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Infrastructure.Services;
using Dreamrosia.Koin.Infrastructure.Services.Identity;
using Dreamrosia.Koin.Infrastructure.Shared.Services;
using Dreamrosia.Koin.Server.Localization;
using Dreamrosia.Koin.Server.Managers.Preferences;
using Dreamrosia.Koin.Server.Permission;
using Dreamrosia.Koin.Server.Schedules;
using Dreamrosia.Koin.Server.Services;
using Dreamrosia.Koin.Server.Settings;
using Dreamrosia.Koin.Shared.Constants.Localization;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Interfaces.Services;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Services;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Quartz;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();

            await SetCultureFromServerPreferenceAsync(serviceProvider);

            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();

            await serviceProvider.DisposeAsync();

            return localizer;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();

            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!

                var preference = await storageService.GetPreference() as ServerPreference;

                CultureInfo culture = new CultureInfo(preference is null ?
                                                      LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US" :
                                                      preference.LanguageCode);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));

            return services;
        }

        internal static ServerConfiguration GetServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(ServerConfiguration));

            services.Configure<ServerConfiguration>(section);

            return section.Get<ServerConfiguration>();
        }

        internal static UPbitConfiguration GetUPbitConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(UPbitConfiguration));

            services.Configure<UPbitConfiguration>(section);

            return section.Get<UPbitConfiguration>();
        }

        internal static KakaoConfiguration GetKakaoConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(KakaoConfiguration));

            services.Configure<KakaoConfiguration>(section);

            return section.Get<KakaoConfiguration>();
        }

        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services.AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                    .Configure<SystemTextJsonOptions>(configureOptions =>
                    {
                        if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        {
                            configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                        }

                        if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(DoubleConverter)))
                        {
                            configureOptions.JsonSerializerOptions.Converters.Add(new DoubleConverter());
                        }
                    });

            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it

            return services;
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlazorHeroContext>(options => options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                                                                                 ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")),
                                                                                 o => o.SchemaBehavior(MySqlSchemaBehavior.Translate, (schema, table) => $"{schema}_{table}")
                                                                                       .EnableRetryOnFailure()),
                                                    ServiceLifetime.Transient)
                    .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

            return services;
        }

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                    .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                    .AddIdentity<BlazorHeroUser, BlazorHeroRole>(options =>
                    {
                        options.Password.RequiredLength = 6;
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<BlazorHeroContext>()
                    .AddDefaultTokenProviders();

            return services;
        }

        internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddTransient<IMailService, SMTPMailService>();

            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IRoleClaimService, RoleClaimService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IAuditService, AuditService>();
            services.AddScoped<IExcelService, ExcelService>();

            services.AddTransient<IMACDService, MACDService>();

            services.AddTransient<ICandleService, CandleService>();
            services.AddTransient<ICrixService, CrixService>();
            services.AddTransient<ISeasonSignalService, SeasonSignalService>();
            services.AddTransient<ISymbolService, SymbolService>();
            services.AddTransient<IDelistingSymbolService, DelistingSymbolService>();
            services.AddTransient<IUnlistedSymbolService, UnlistedSymbolService>();
            services.AddTransient<IMarketIndexService, MarketIndexService>();

            services.AddTransient<IMiningBotService, MiningBotService>();

            services.AddTransient<IAssetService, AssetService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<ITransferService, TransferService>();
            services.AddTransient<ITradingTermsService, TradingTermsService>();
            services.AddTransient<IUPbitKeyService, UPbitKeyService>();

            services.AddTransient<IBankingTransactionService, BankingTransactionService>();
            services.AddTransient<IPointService, PointService>();

            services.AddTransient<IBackTestingService, BackTestingService>();
            services.AddTransient<IBackTestingTraderService, BackTestingTraderService>();

#if DEBUG
            services.AddTransient<ITestService, TestService>();
#endif
            return services;
        }

        internal static IServiceCollection AddMultiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var server = services.GetServerConfiguration(configuration);
            var kakao = services.GetKakaoConfiguration(configuration);

            var key = Encoding.ASCII.GetBytes(server.Secret);

            services.AddAuthentication(authentication =>
                    {
                        authentication.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddCookie(options =>
                    {
                        options.LoginPath = "/signin";
                        options.LogoutPath = "/signout";
                        options.CookieManager = new ChunkingCookieManager();
                    })
                    .AddKakaoTalk(options =>
                    {
                        options.ClientId = kakao.ClientId;
                        options.ClientSecret = kakao.ClientSecret;
                        options.SaveTokens = true;
                        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.ClaimActions.MapJsonSubKey(ApplicationClaimTypes.KoreanName, "kakao_account", "name");
                        options.ClaimActions.MapJsonSubKey(ApplicationClaimTypes.ProfielImange, "properties", ApplicationClaimTypes.ProfielImange);
                        options.ClaimActions.MapJsonSubKey(ApplicationClaimTypes.ThumbnailImage, "properties", ApplicationClaimTypes.ThumbnailImage);

                        // pre defined in KakaoTalkAuthenticationOptions
                        //options.ClaimActions.MapJsonSubKey(ClaimTypes.Email, "kakao_account", "email");
                        //options.ClaimActions.MapJsonSubKey(ClaimTypes.DateOfBirth, "kakao_account", "birthday");
                        //options.ClaimActions.MapJsonSubKey(ClaimTypes.Gender, "kakao_account", "gender");
                        //options.ClaimActions.MapJsonSubKey(ClaimTypes.MobilePhone, "kakao_account", "phone_number");
                        //options.ClaimActions.MapJsonSubKey(Claims.AgeRange, "kakao_account", "age_range");
                        //options.ClaimActions.MapJsonSubKey(Claims.YearOfBirth, "kakao_account", "birthyear");

                    })
                    .AddJwtBearer(async bearer =>
                    {
                        bearer.RequireHttpsMetadata = false;
                        bearer.SaveToken = true;
                        bearer.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            RoleClaimType = ClaimTypes.Role,
                            ClockSkew = TimeSpan.Zero
                        };

                        //var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);
                        var localizer = await GetRegisteredServerLocalizerAsync<SharedLocalizerResources>(services);

                        bearer.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = c =>
                            {
                                if (c.Exception is SecurityTokenExpiredException)
                                {
                                    c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    c.Response.ContentType = "application/json";

                                    var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));

                                    return c.Response.WriteAsync(result);
                                }
                                else
                                {
                                    c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    c.Response.ContentType = "application/json";

                                    var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));

                                    return c.Response.WriteAsync(result);
                                }
                            },
                            OnChallenge = context =>
                            {
                                context.HandleResponse();

                                if (!context.Response.HasStarted)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                    context.Response.ContentType = "application/json";

                                    var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));

                                    return context.Response.WriteAsync(result);
                                }

                                return Task.CompletedTask;
                            },
                            OnForbidden = context =>
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                                context.Response.ContentType = "application/json";

                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));

                                return context.Response.WriteAsync(result);
                            },
                        };
                    });

            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes()
                                                        .SelectMany(c => c.GetFields(BindingFlags.Public |
                                                                                     BindingFlags.Static |
                                                                                     BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);

                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });

            return services;
        }

        internal static IServiceCollection AddUPbitServices(this IServiceCollection services)
        {
            services.AddTransient<IUPbitCandleService, UPbitCandleService>();
            services.AddTransient<IUPbitCrixService, UPbitCrixService>();
            services.AddTransient<IUPbitSymbolService, UPbitSymbolService>();
            services.AddTransient<IUPbitMarketIndexService, UPbitMarketIndexService>();

            services.AddSingleton<IUPbitTickerService, UPbitTickerService>();

            return services;
        }

        internal static IServiceCollection AddQuartz(this IServiceCollection services, ServerConfiguration config)
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                q.AddJobAndTrigger<MiningBotJob>();

                if (config.Mode == ServerModes.Server ||
                    config.Mode == ServerModes.Agent)
                {
                    q.AddJobAndTrigger<MarketJob>();
                    q.AddJobAndTrigger<TickerJob>();
                    //q.AddJobAndTrigger<SeasonSignalJob>();
                    //q.AddJobAndTrigger<MarketIndexJob>();
                    //q.AddJobAndTrigger<DelistingSymbolJob>();
                    //q.AddJobAndTrigger<UnlistedSymbolJob>();
                    q.AddJobAndTrigger<PointJob>();
                }
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

            if (typeof(T) == typeof(MarketJob))
            {
                cronSchedule = "5 * * * * ?";
            }
            else if (typeof(T) == typeof(TickerJob))
            {
                cronSchedule = "*/2 * * * * ?";
            }
            //else if (typeof(T) == typeof(MarketIndexJob))
            //{
            //    cronSchedule = "10 * * * * ?";
            //}
            //else if (typeof(T) == typeof(DelistingSymbolJob))
            //{
            //    cronSchedule = "15 * * * * ?";
            //}
            //else if (typeof(T) == typeof(UnlistedSymbolJob))
            //{
            //    cronSchedule = "20 * * * * ?";
            //}
            else if (typeof(T) == typeof(PointJob))
            {
                cronSchedule = "30 0 0 * * ?";
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