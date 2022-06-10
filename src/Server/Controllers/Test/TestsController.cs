using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : BaseApiController<TestsController>
    {
        private readonly ITestService _testService;

        public TestsController(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet("candles")]
        public async Task<IActionResult> GetUPbitCandles()
        {
            var response = await _testService.GetUPbitCandlesAsync();

            return Ok(response);
        }

        [HttpGet("crixes")]
        public async Task<IActionResult> GetUPbitCrixes()
        {
            var response = await _testService.GetUPbitCrixesAsync();

            return Ok(response);
        }

        [HttpGet("symbols")]
        public async Task<IActionResult> GetUPbitSymbols()
        {
            var response = await _testService.GetUPbitSymbolsAsync();

            return Ok(response);
        }

        [HttpGet("orders/{userId}")]
        public async Task<IActionResult> GetUPbitOrders(string userId)
        {
            var response = await _testService.GetUPbitOrdersAsync(userId);

            return Ok(response);
        }

        [HttpGet("positions/{userId}")]
        public async Task<IActionResult> GetUPbitPositions(string userId)
        {
            var response = await _testService.GetUPbitPositionsAsync(userId);

            return Ok(response);
        }

        [HttpGet("deposits/{userId}")]
        public async Task<IActionResult> GetUPbitDeposits(string userId)
        {
            var response = await _testService.GetUPbitTransfersAsync(userId, TransferType.deposit);

            return Ok(response);
        }

        [HttpGet("withdraws/{userId}")]
        public async Task<IActionResult> GetUPbitWithdraws(string userId)
        {
            var response = await _testService.GetUPbitTransfersAsync(userId, TransferType.withdraw);

            return Ok(response);
        }

        [HttpGet("seasonsignals/{userId}")]
        public async Task<IActionResult> GetSeasonSignals(string userId)
        {
            var response = await _testService.GetSeasonSignalsAsync(userId);

            return Ok(response);
        }

        [HttpGet("seasonsignals/update/{userId}")]
        public async Task<IActionResult> UpdateSeasonSignals(string userId)
        {
            var response = await _testService.UpdateSeasonSignalsAsync(userId);

            return Ok(response);
        }
    }
}