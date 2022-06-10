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
    public class MockTradingsController : BaseApiController<MockTradingsController>
    {
        private readonly IMockTradingService _mockTradingService;

        public MockTradingsController(IMockTradingService mockTradingService)
        {
            _mockTradingService = mockTradingService;
        }


        [Authorize(Policy = Permissions.MockTradings.BackTest)]
        [HttpPost("backtest")]
        public async Task<IActionResult> GetBackTestingAsync(BackTestRequestDto model)
        {
            var response = await _mockTradingService.GetBackTestingAsync(model);

            return Ok(response);
        }
    }
}