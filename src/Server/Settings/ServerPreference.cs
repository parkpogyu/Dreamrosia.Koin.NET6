using Dreamrosia.Koin.Shared.Constants.Localization;
using Dreamrosia.Koin.Shared.Settings;
using System.Linq;

namespace Dreamrosia.Koin.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";

        //TODO - add server preferences
    }
}