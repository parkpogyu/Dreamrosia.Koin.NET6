using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettlementsController : BaseApiController<SettlementsController>
    {
        private readonly IBankingTransactionService _settlementService;
        private readonly IPointService _pointService;

        public SettlementsController(IBankingTransactionService settlementService,
                                     IPointService pointService)
        {
            _settlementService = settlementService;
            _pointService = pointService;
        }

        [Authorize(Policy = Permissions.BankingTransactions.View)]
        [HttpPost("bankingtransactions")]
        public async Task<IActionResult> GetBankingTransactions(BankingTransactionsRequestDto model)
        {
            var response = await _settlementService.GetBankingTransactionsAsync(model);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.BankingTransactions.Import)]
        [HttpPost("bankingtransactions/import")]
        public async Task<IActionResult> ImportBankingTransactions(UploadRequest model)
        {
            var response = await _settlementService.ImportBankingTransactionsAsync(model);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.Points.View)]
        [HttpPost("points/{userId}")]
        public async Task<IActionResult> GetPoints(PointsRequestDto model)
        {
            var response = await _pointService.GetPointsAsync(model);

            return Ok(response);
        }
    }
}