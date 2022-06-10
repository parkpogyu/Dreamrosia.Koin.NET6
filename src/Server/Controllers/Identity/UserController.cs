using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers.Identity
{
    [Authorize]
    [Route("api/identity/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK</returns>
        //[Authorize(Policy = Permissions.Users.View)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(string userId)
        {
            var user = await _userService.GetAsync(userId);

            return Ok(user);
        }

        [HttpGet("detail/{userId}")]
        public async Task<IActionResult> GetDetail(string userId)
        {
            var user = await _userService.GetDetailAsync(userId);

            return Ok(user);
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet]
        public async Task<IActionResult> GetSummarise(DateTime? head, DateTime? rear)
        {
            rear = rear is null ? DateTime.Now.Date : Convert.ToDateTime(rear);

            var users = await _userService.GetSummariseAsync(Convert.ToDateTime(head), Convert.ToDateTime(rear));

            return Ok(users);
        }

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowers(string id, DateTime? head, DateTime? rear)
        {
            rear = rear is null ? DateTime.Now.Date : Convert.ToDateTime(rear);

            var users = await _userService.GetFollowersAsync(id, Convert.ToDateTime(head), Convert.ToDateTime(rear));

            return Ok(users);
        }

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("boasters")]
        public async Task<IActionResult> GetBoasters(DateTime? head, DateTime? rear)
        {
            rear = rear is null ? DateTime.Now.Date : Convert.ToDateTime(rear);

            var users = await _userService.GetBoastersAsync(Convert.ToDateTime(head), Convert.ToDateTime(rear));

            return Ok(users);
        }

        [HttpPost("recommender/{id}")]
        public async Task<IActionResult> GetRecommender(RecommenderDto model)
        {
            var recommenders = await _userService.GetRecommenderAsync(model);

            return Ok(recommenders);
        }

        [HttpPost("recommender/update/{id}")]
        public async Task<IActionResult> UpdateRecommender(RecommenderDto model)
        {
            return Ok(await _userService.UpdateRecommenderAsync(model));
        }

        [HttpGet("accountholder/{userCode}")]
        public async Task<IActionResult> GetAccountHolder(string userCode)
        {
            var recommenders = await _userService.GetAccountHolderAsync(userCode);

            return Ok(recommenders);
        }

        [HttpPost("membership/{userId}")]
        public async Task<ActionResult> ChangeMembership(MembershipDto model)
        {
            var response = await _userService.ChangeMembershipAsync(model);

            return Ok(response);
        }

        /// <summary>
        /// Get User Roles By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRolesAsync(string id)
        {
            var userRoles = await _userService.GetRolesAsync(id);
            return Ok(userRoles);
        }

        /// <summary>
        /// Update Roles for User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            return Ok(await _userService.UpdateRolesAsync(request));
        }

        /// <summary>
        /// Register a User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];

            return Ok(await _userService.RegisterAsync(request, origin));
        }

        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            return Ok(await _userService.ConfirmEmailAsync(userId, code));
        }

        /// <summary>
        /// Toggle User Status (Activate and Deactivate)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            return Ok(await _userService.ToggleUserStatusAsync(request));
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            var data = await _userService.ExportToExcelAsync(searchString);
            return Ok(data);
        }
    }
}