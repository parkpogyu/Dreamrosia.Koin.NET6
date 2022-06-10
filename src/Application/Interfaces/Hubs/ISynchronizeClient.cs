using Dreamrosia.Koin.Application.DTO;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Hubs
{
    public interface ISynchronizeClient
    {
        Task ReceiveTicker(TickerDto ticker);

        Task ReceivePositions(string userId, PositionsDto positions);
    }
}
