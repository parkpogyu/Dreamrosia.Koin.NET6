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
    public class BackTestingsController : BaseApiController<BackTestingsController>
    {
        private readonly IBackTestingService _backTestingService;

        public BackTestingsController(IBackTestingService backTestingService)
        {
            _backTestingService = backTestingService;
        }


        [Authorize(Policy = Permissions.BackTestings.BackTesting)]
        [HttpPost]
        public async Task<IActionResult> GetBackTestingAsync(BackTestingRequestDto model)
        {
            var response = await _backTestingService.GetBackTestingAsync(model);

            return Ok(response);
        }
    }
}