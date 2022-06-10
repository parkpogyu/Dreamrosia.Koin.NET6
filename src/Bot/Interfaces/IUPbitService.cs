using System.Threading.Tasks;

namespace Dreamrosia.Koin.Bot.Interfaces
{
    public interface IUPbitService
    {
        Task GetPositionsAsync();

        Task GetCompletedOrdersAsync();

        Task GetWaitingOrdersAsync();

        Task GetDepositsAsync();

        Task GetWithdrawsAsync();
    }
}