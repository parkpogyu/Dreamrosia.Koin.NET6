using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<IResult<IEnumerable<OrderDto>>> GetOrdersAsync(OrdersRequestDto model);
        Task<IResult<OrderDto>> GetLastOrderAsync(string userId);
        Task<IResult> SaveOrdersAsync(string userId, IEnumerable<OrderDto> models, bool done);
    }
}