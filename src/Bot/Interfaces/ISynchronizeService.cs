using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Interfaces
{
    public interface ISynchronizeService
    {
        Task ConnectHubAsync();

        Task GetTradingTermsAsync();

        Task SavePositionsAsync();

        Task SaveOrdersAsync();

        Task SaveTransfersAsync();

        Task OccurredFatalErrorAsync(string code, string message);
    }
}