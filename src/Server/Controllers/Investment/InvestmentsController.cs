using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentsController : BaseApiController<InvestmentsController>
    {
        private readonly IAssetService _assetService;
        private readonly IOrderService _orderService;
        private readonly IPositionService _positionService;
        private readonly ITransferService _transferService;

        public InvestmentsController(IAssetService assetService,
                                     IOrderService orderService,
                                     IPositionService positionService,
                                     ITransferService transferService)
        {
            _assetService = assetService;
            _orderService = orderService;
            _positionService = positionService;
            _transferService = transferService;
        }

        [Authorize(Policy = Permissions.Assets.View)]
        [HttpGet("assets/{userId}")]
        public async Task<IActionResult> GetAssets(string userId)
        {
            var response = await _assetService.GetAssetsAsync(userId);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Orders.View)]
        [HttpPost("orders/{userId}")]
        public async Task<IActionResult> GetOrders(OrdersRequestDto model)
        {
            var response = await _orderService.GetOrdersAsync(model);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Positions.View)]
        [HttpGet("positions/{userId}")]
        public async Task<IActionResult> GetPositions(string userId)
        {
            var response = await _positionService.GetPositionsAsync(userId);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Transfers.View)]
        [HttpPost("transfers/{userId}")]
        public async Task<IActionResult> GetTransfers(TransfersRequestDto model)
        {
            var response = await _transferService.GetTransfersAsync(model);

            return Ok(response);
        }
    }
}