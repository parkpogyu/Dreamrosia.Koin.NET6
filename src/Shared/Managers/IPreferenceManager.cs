using Dreamrosia.Koin.Shared.Settings;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}