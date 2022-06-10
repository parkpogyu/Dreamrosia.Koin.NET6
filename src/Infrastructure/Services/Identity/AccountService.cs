using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Account;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Requests.Mail;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly SignInManager<BlazorHeroUser> _signInManager;
        private readonly BlazorHeroContext _context;
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public AccountService(UserManager<BlazorHeroUser> userManager,
                              SignInManager<BlazorHeroUser> signInManager,
                              BlazorHeroContext context,
                              IUnitOfWork<string> unitOfWork,
                              IUploadService uploadService,
                              IMapper mapper,
                              IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
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

        public async Task<IResult<UserProfileDto>> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

            var result = _mapper.Map<UserProfileDto>(user);

            return await Result<UserProfileDto>.SuccessAsync(result);
        }

        public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId)
        {
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already used."], request.PhoneNumber));
                }
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null || userWithSameEmail.Id == userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return await Result.FailAsync(_localizer["User Not Found!"]);
                }
                user.NickName = request.NickName;
                user.PhoneNumber = request.PhoneNumber;
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (request.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                }
                var identityResult = await _userManager.UpdateAsync(user);
                var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
                await _signInManager.RefreshSignInAsync(user);
                return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
            }
            else
            {
                return await Result.FailAsync(string.Format(_localizer["Email {0} is already used."], request.Email));
            }
        }

        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result<string>.FailAsync(_localizer["User Not Found"]);
            }
            return await Result<string>.SuccessAsync(data: user.ProfileImage);
        }

        public async Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return await Result<string>.FailAsync(message: _localizer["User Not Found"]);
            var filePath = _uploadService.UploadAsync(request);
            user.ProfileImage = filePath;
            var identityResult = await _userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            return identityResult.Succeeded ? await Result<string>.SuccessAsync(data: filePath) : await Result<string>.FailAsync(errors);
        }
    }
}