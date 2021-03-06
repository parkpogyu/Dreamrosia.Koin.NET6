using Dreamrosia.Koin.Shared.Constants.Localization;
using Dreamrosia.Koin.Shared.Settings;
using System.Linq;

namespace Dreamrosia.Koin.Client.Infrastructure.Settings
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; }
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
    }
}