using AspNet.Security.OAuth.KakaoTalk;
using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Server.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _identityService;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;
        private readonly KakaoConfiguration _kakaoConfig;
        private readonly SignInManager<BlazorHeroUser> _signInManager;

        public TokenController(ITokenService identityService,
                               IUserService userService,
                               IAccountService accountService,
                               ICurrentUserService currentUserService,
                               IOptions<KakaoConfiguration> kakaoConfig,
                               SignInManager<BlazorHeroUser> signInManager)
        {
            _identityService = identityService;
            _userService = userService;
            _currentUserService = currentUserService;
            _accountService = accountService;
            _kakaoConfig = kakaoConfig.Value;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<IActionResult> Get(TokenRequest model)
        {
            var response = await _identityService.LoginAsync(model);

            return Ok(response);
        }

        [HttpGet("kakao")]
        public async Task<IActionResult> GetKakao()
        {
            var response = await _identityService.KakaoLoginAsync();

            return Ok(response);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            var response = await _identityService.GetRefreshTokenAsync(model);

            return Ok(response);
        }

        [HttpGet("kakao-signin")]
        public IActionResult KakaoSignin()
        {
            var uri = Url.Action(nameof(SigninKakaoTalkCallback), "Token");

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(KakaoTalkAuthenticationDefaults.Issuer, uri);

            return Challenge(properties, KakaoTalkAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> SigninKakaoTalkCallback()
        {
            var result = await _accountService.RegisterOrUpdateKakaoUserAsync();

            return Redirect("/");
        }

        [HttpGet("kakao-signout"), AllowAnonymous]
        public IActionResult KakaoSignout()
        {
            var redirectUri = $"{Request.Scheme}://{Request.Host}";
            var signoutUri = $"{KakaoConfiguration.SignoutUrl}?client_id={_kakaoConfig.ClientId}&logout_redirect_uri={redirectUri}";

            return Redirect(signoutUri);
        }
    }
}