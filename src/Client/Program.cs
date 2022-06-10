using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers.Preferences;
using Dreamrosia.Koin.Client.Infrastructure.Settings;
using Dreamrosia.Koin.Shared.Constants.Localization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .AddRootComponents()
                          .AddClientServices();

            var host = builder.Build();

            var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();

            if (storageService != null)
            {
                var preference = await storageService.GetPreference() as ClientPreference;

                CultureInfo culture = new CultureInfo(preference is null ?
                                                       LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US" :
                                                       preference.LanguageCode);

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;


                //format.ShortDatePattern = "yyyy-MM-dd";
                //format.ShortTimePattern = "HH:mm:ss";
                //format.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";

                CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
                CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";
                CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat.YearMonthPattern = "yyyy-MM";

                CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
                CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";
                CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.YearMonthPattern = "yyyy-MM";
            }

            await builder.Build().RunAsync();
        }
    }
}