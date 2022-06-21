using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
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
        private readonly ICurrentUserService _currentUser;

        public UserController(IUserService userService,
                              ICurrentUserService currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
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

        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("subscription/{userId}")]
        public async Task<IActionResult> GetSubscription(string userId)
        {
            var user = await _userService.GetSubscriptionAsync(userId);

            return Ok(user);
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet]
        public async Task<IActionResult> GetSummaries(DateTime? head, DateTime? rear)
        {
            rear = rear is null ? DateTime.Now.Date : Convert.ToDateTime(rear);

            var users = await _userService.GetSummariesAsync(Convert.ToDateTime(head), Convert.ToDateTime(rear));

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

        [HttpGet("profile/{userId}")]
        public async Task<ActionResult> GetProfile(string userId)
        {
            var response = await _userService.GetProfileAsync(userId);

            return Ok(response);
        }

        /// <summary>
        /// Update Profile
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPut(nameof(UpdateProfile))]
        public async Task<ActionResult> UpdateProfile(UpdateProfileRequest model)
        {
            var response = await _userService.UpdateProfileAsync(model, _currentUser.UserId);
            return Ok(response);
        }

        /// <summary>
        /// Get Profile picture by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK </returns>
        [HttpGet("profile-picture/{userId}")]
        [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 60)]
        public async Task<IActionResult> GetProfilePictureAsync(string userId)
        {
            return Ok(await _userService.GetProfilePictureAsync(userId));
        }

        /// <summary>
        /// Update Profile Picture
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("profile-picture/{userId}")]
        public async Task<IActionResult> UpdateProfilePictureAsync(UpdateProfilePictureRequest request)
        {
            return Ok(await _userService.UpdateProfilePictureAsync(request, _currentUser.UserId));
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