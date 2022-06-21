using AutoMapper;
using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Exceptions;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Requests.Mail;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Role;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly SignInManager<BlazorHeroUser> _signInManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;
        private readonly BlazorHeroContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public AccountService(UserManager<BlazorHeroUser> userManager,
                              SignInManager<BlazorHeroUser> signInManager,
                              RoleManager<BlazorHeroRole> roleManager,
                              BlazorHeroContext context,
                              IUnitOfWork<string> unitOfWork,
                              ICurrentUserService currentUserService,
                              IMapper mapper,
                              IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync(_localizer["User Not Found!"]);
            }

            var identityResult = await this._userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            var mailRequest = new MailRequest
            {
                Body = string.Format(_localizer["Please reset your password by <a href='{0}>clicking here</a>."], HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = _localizer["Reset Password"],
                To = request.Email
            };
            //BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }
        }

        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            var user = new BlazorHeroUser
            {
                Email = request.Email,
                NickName = request.NickName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoConfirmEmail
            };

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
                }
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
                    if (!request.AutoConfirmEmail)
                    {
                        var verificationUri = await SendVerificationEmail(user, origin);
                        var mailRequest = new MailRequest
                        {
                            From = "mail@codewithmukesh.com",
                            To = user.Email,
                            Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUri),
                            Subject = _localizer["Confirm Registration"]
                        };
                        //BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
                        return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.NickName));
                    }
                    return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered."], user.NickName));
                }
                else
                {
                    return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
                }
            }
            else
            {
                return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
            }
        }

        private async Task<string> SendVerificationEmail(BlazorHeroUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/identity/user/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public async Task<IResult> RegisterOrUpdateKakaoUserAsync()
        {
            var external = await _signInManager.GetExternalLoginInfoAsync();

            if (external == null)
            {
                return await Result<UserDto>.FailAsync(_localizer["You are not Authorized."]);
            }

            var user = await _userManager.FindByEmailAsync(external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Email)?.Value);

            if (user is null)
            {
                user = new BlazorHeroUser
                {
                    Email = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Email)?.Value,
                    PhoneNumber = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.MobilePhone)?.Value,
                    UserName = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Email)?.Value,
                    NickName = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Name)?.Value,
                    KoreanName = external.Principal.Claims.SingleOrDefault(f => f.Type == ApplicationClaimTypes.KoreanName)?.Value,
                    ProfileImage = external.Principal.Claims.SingleOrDefault(f => f.Type == ApplicationClaimTypes.ThumbnailImage)?.Value,
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var access_token = external.AuthenticationTokens.SingleOrDefault(f => f.Name.Equals(KakaoConfiguration.TockenName));

                    /// 오류 시 처리방안 모색
                    await _userManager.AddLoginAsync(user, new UserLoginInfo(external.LoginProvider, external.ProviderKey, external.ProviderDisplayName));
                    await _userManager.SetAuthenticationTokenAsync(user, external.LoginProvider, KakaoConfiguration.TockenName, access_token?.Value);
                    await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);

                    _context.Database.ExecuteSqlRaw($"CALL PRC_Initialize_User_Terms('{user.Id}')");

                    return await Result<UserDto>.SuccessAsync(_localizer["User Registered"]);
                }
                else
                {
                    return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
                }
            }
            else
            {
                user.Email = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Email)?.Value;
                user.PhoneNumber = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.MobilePhone)?.Value;
                user.UserName = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Email)?.Value;
                user.NickName = external.Principal.Claims.SingleOrDefault(f => f.Type == ClaimTypes.Name)?.Value;
                user.KoreanName = external.Principal.Claims.SingleOrDefault(f => f.Type == ApplicationClaimTypes.KoreanName)?.Value;
                user.ProfileImage = external.Principal.Claims.SingleOrDefault(f => f.Type == ApplicationClaimTypes.ThumbnailImage)?.Value;

                var tocken_name = KakaoConfiguration.TockenName;    // "access_token";
                var access_token = external.AuthenticationTokens.SingleOrDefault(f => f.Name.Equals(tocken_name));

                await _userManager.SetAuthenticationTokenAsync(user, external.LoginProvider, tocken_name, access_token?.Value);

                var result = await _userManager.UpdateAsync(user);

                return await Result<UserDto>.SuccessAsync(_localizer["User Already Registered"]);
            }
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
            if (isAdmin)
            {
                return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                var identityResult = await _userManager.UpdateAsync(user);
            }
            return await Result.SuccessAsync();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            var viewModel = new List<UserRoleModel>();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = viewModel };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user.Email == "mjtobi@gmail.com")
            {
                return await Result.FailAsync(_localizer["Not Allowed."]);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
            {
                var tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == RoleConstants.AdministratorRole);
                var userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
                if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
                {
                    return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email));
            }
            else
            {
                throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
            }
        }
    }
}