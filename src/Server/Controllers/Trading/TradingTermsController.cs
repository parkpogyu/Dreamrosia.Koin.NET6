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
    public class TradingTermsController : BaseApiController<TradingTermsController>
    {
        private readonly ITradingTermsService _tradingTermsService;

        public TradingTermsController(ITradingTermsService tradingTermsService)
        {
            _tradingTermsService = tradingTermsService;
        }

        [Authorize(Policy = Permissions.TradingTerms.View)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTradingTermsByUserId(string userId)
        {
            var response = await _tradingTermsService.GetTradingTermsAsync(userId);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.TradingTerms.Edit)]
        [HttpPost("update/{userId}")]
        public async Task<IActionResult> UpdateTraginTerms(TradingTermsDto model)
        {
            var response = await _tradingTermsService.UpdateTradingTermsAsync(model);

            return Ok(response);
        }
    }
}