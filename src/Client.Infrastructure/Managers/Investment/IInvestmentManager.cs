using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface IInvestmentManager : IManager
    {
        Task<IResult<AssetReportDto>> GetAssetsAsync(string userId);

        Task<IResult<IEnumerable<OrderDto>>> GetOrdersAsync(OrdersRequestDto model);

        Task<IResult<PositionsDto>> GetPositionsAsync(string userId);

        Task<IResult<IEnumerable<TransferDto>>> GetTransfersAsync(TransfersRequestDto model);
    }
}