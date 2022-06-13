using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UPbitKeysController : BaseApiController<UPbitKeysController>
    {
        private readonly IUPbitKeyService _upbitKeyService;

        private readonly UPbitConfiguration _configuration;

        public UPbitKeysController(IUPbitKeyService upbitKeyService,
                                   IOptions<UPbitConfiguration> config)
        {
            _upbitKeyService = upbitKeyService;
            _configuration = config.Value;
        }

        [Authorize(Policy = Permissions.UPbitKeys.View)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUPbitKeyByUserId(string userId)
        {
            var response = await _upbitKeyService.GetUPbitKeyAsync(userId);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.UPbitKeys.View)]
        [HttpGet("allowedips")]
        public async Task<IActionResult> GetAllowdIPs()
        {
            var response = await Result<IEnumerable<string>>.SuccessAsync(_configuration.AllowedIPs);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.UPbitKeys.Edit)]
        [HttpPost("update/{userId}")]
        public async Task<IActionResult> UpdateUPbitKey(UPbitKeyDto model)
        {
            var response = await _upbitKeyService.UpdateUPbitKeyAsync(model);

            return Ok(response);
        }

        [Authorize(Policy = Permissions.UPbitKeys.Edit)]
        [HttpPost("test/{userId}")]
        public async Task<IActionResult> TestUPbitKey(UPbitKeyTestDto model)
        {
            var response = await _upbitKeyService.TestUPbitKeyAsync(model);

            return Ok(response);
        }
    }
}