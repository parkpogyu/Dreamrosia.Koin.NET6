using AutoMapper;
using Dreamrosia.Koin.Application.Configurations;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Exceptions;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Application.Requests.Mail;
using Dreamrosia.Koin.Application.Responses.Identity;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Infrastructure.Specifications;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Role;
using Dreamrosia.Koin.Shared.Interfaces.Services;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;
        private readonly SignInManager<BlazorHeroUser> _signInManager;
        private readonly BlazorHeroContext _context;
        private readonly IUnitOfWork<string> _strUnitOfWork;
        private readonly IUnitOfWork<int> _intUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExcelService _excelService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public UserService(UserManager<BlazorHeroUser> userManager,
                           RoleManager<BlazorHeroRole> roleManager,
                           SignInManager<BlazorHeroUser> signInManager,
                           BlazorHeroContext context,
                           IUnitOfWork<string> strUnitOfWork,
                           IUnitOfWork<int> intUnitOfWork,
                           ICurrentUserService currentUserService,
                           IMailService mailService,
                           IExcelService excelService,
                           IMapper mapper,
                           ILogger<UserService> logger,
                           IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _strUnitOfWork = strUnitOfWork;
            _intUnitOfWork = intUnitOfWork;
            _currentUserService = currentUserService;
            _excelService = excelService;
            _mailService = mailService;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            try
            {
                var item = await _context.Users
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (item is null)
                {
                    return await Result<UserResponse>.FailAsync(_localizer["User Not Found!"]);
                }

                return await Result<UserResponse>.SuccessAsync(_mapper.Map<UserResponse>(item));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UserResponse>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<UserDetailDto>> GetDetailAsync(string userId)
        {
            try
            {
                var user = (from usr in _context.Users
                                                .AsNoTracking()
                                                .Where(f => f.Id.Equals(userId))
                                                .Include(i => i.Subscription)
                                                .Include(i => i.Memberships)
                                                .Include(i => i.MiningBotTicket)
                                                .Include(i => i.TradingTerms)
                                                .AsEnumerable()
                            from usrcode in _context.UserLogins
                                                    .AsNoTracking()
                                                    .Where(f => f.UserId.Equals(usr.Id))
                                                    .AsEnumerable()
                            from rec in _context.Users
                                                .AsNoTracking()
                                                .Where(f => f.Id.Equals(usr.Subscription.RecommenderId)).DefaultIfEmpty()
                                                .AsEnumerable()
                            from reccode in _context.UserLogins
                                                    .AsNoTracking()
                                                    .Where(f => f.UserId.Equals(usr.Subscription.RecommenderId)).DefaultIfEmpty()
                                                    .AsEnumerable()
                            select ((Func<UserDetailDto>)(() =>
                            {
                                var item = _mapper.Map<UserDetailDto>(usr);

                                var membership = usr.Memberships.OrderByDescending(o => o.CreatedOn).First();

                                item.UserCode = usrcode.ProviderKey;
                                item.Membership = _mapper.Map<MembershipDto>(membership);

                                item.AutoTrading = usr.TradingTerms.AutoTrading;
                                item.TimeFrame = usr.TradingTerms.TimeFrame;
                                item.IsAssignedBot = usr.MiningBotTicket is null ? false : true;

                                item.Recommender = _mapper.Map<UserSummaryDto>(rec);

                                if (item.Recommender is not null)
                                {
                                    item.Recommender.UserCode = reccode?.ProviderKey;
                                }

                                return item;
                            }))()).SingleOrDefault();

                if (user is null)
                {
                    return await Result<UserDetailDto>.FailAsync(_localizer["User Not Found!"]);
                }

                return await Result<UserDetailDto>.SuccessAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UserDetailDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        private IEnumerable<UserSummaryDto> GetUsers(DateTime head, DateTime rear, string userId = null, bool? goBoast = null)
        {
            var items = (from usr in _context.Users
                                             .AsNoTracking()
                                             .Include(i => i.Subscription)
                                             .Include(i => i.Memberships)
                                             .Include(i => i.MiningBotTicket)
                                             .Include(i => i.TradingTerms)
                                             .Where(f => (string.IsNullOrEmpty(userId) ? true : userId.Equals(f.Subscription.RecommenderId)) &&
                                                         (head.Date <= f.CreatedOn && f.CreatedOn < rear.Date.AddDays(1)) &&
                                                         (goBoast == null ? true : f.Subscription.GoBoast == goBoast))
                                             .AsEnumerable()
                         from ext in _context.UserLogins
                                             .AsNoTracking()
                                             .Where(f => f.UserId.Equals(usr.Id))
                                             .AsEnumerable()
                         orderby usr.CreatedOn descending
                         select ((Func<UserSummaryDto>)(() =>
                         {
                             var item = _mapper.Map<UserSummaryDto>(usr);

                             var membership = usr.Memberships.OrderByDescending(o => o.CreatedOn).First();

                             item.UserCode = ext.ProviderKey;
                             item.MembershipLevel = membership.Level;
                             item.MaximumAsset = membership.MaximumAsset;

                             item.AutoTrading = usr.TradingTerms.AutoTrading;
                             item.TimeFrame = usr.TradingTerms.TimeFrame;
                             item.IsAssignedBot = usr.MiningBotTicket is null ? false : true;

                             return item;
                         }))()).ToArray();

            return items;
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariseAsync(DateTime head, DateTime rear)
        {
            try
            {
                var items = GetUsers(head, rear);

                var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);

                var isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole);

                if (isAdmin)
                {
                    foreach (var item in items)
                    {
                        var roles = await GetRolesAsync(item.Id);

                        if (roles.Succeeded)
                        {
                            item.RolesDescription = string.Join(",", roles.Data
                                                                          .UserRoles
                                                                          .Where(f => f.Selected)
                                                                          .Select(f => f.RoleName));
                        }
                    }
                }

                return await Result<IEnumerable<UserSummaryDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<UserSummaryDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetFollowersAsync(string userId, DateTime head, DateTime rear)
        {
            try
            {
                var items = GetUsers(head, rear, userId);

                return await Result<IEnumerable<UserSummaryDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<UserSummaryDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetBoastersAsync(DateTime head, DateTime rear)
        {
            try
            {
                var items = GetUsers(head, rear, null, true);

                return await Result<IEnumerable<UserSummaryDto>>.SuccessAsync(items);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<UserSummaryDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<UserResponse>> GetRecommenderAsync(RecommenderDto model)
        {
            try
            {
                var items = (from usr in _context.Users
                                                 .AsNoTracking()
                                                 .Where(f => f.Id.Equals(model.UserId))
                                                 .AsEnumerable()
                             from ext in _context.UserLogins
                                                 .AsNoTracking()
                                                 .Where(f => f.ProviderKey.Equals(model.UserCode))
                                                 .AsEnumerable()
                             from rec in _context.Users
                                                 .AsNoTracking()
                                                 .Where(f => !f.Id.Equals(model.UserId) &&
                                                              f.Id.Equals(ext.UserId) &&
                                                              f.CreatedOn < usr.CreatedOn).DefaultIfEmpty()
                                                 .Include(i => i.Subscription)
                                                 .AsEnumerable()
                             select ((Func<UserResponse>)(() =>
                             {
                                 var item = _mapper.Map<UserResponse>(rec);

                                 return item;
                             }))()).SingleOrDefault();

                return await Result<UserResponse>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UserResponse>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> UpdateRecommenderAsync(RecommenderDto model)
        {
            try
            {
                var item = await _context.Subscriptions
                                         .SingleOrDefaultAsync(p => p.Id.Equals(model.UserId));

                if (item is null)
                {
                    return await Result.FailAsync(_localizer["User Not Found!"]);
                }
                else
                {
                    item.RecommenderId = model.RecommenderId;

                    await _strUnitOfWork.Repository<Subscription>().UpdateAsync(item);
                    await _strUnitOfWork.Commit(new CancellationToken());

                    return await Result<string>.SuccessAsync(model.UserId, string.Format(_localizer["{0} Updated"], _localizer["Subscription.Recommender"]));
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<UserResponse>> GetAccountHolderAsync(string userCode)
        {
            var item = (from ext in _context.UserLogins
                                            .AsNoTracking()
                                            .Where(f => f.ProviderKey.Equals(userCode))
                                            .AsEnumerable()
                        from usr in _context.Users
                                            .AsNoTracking()
                                            .Where(f => f.Id.Equals(ext.UserId))
                                            .Include(f => f.Subscription)
                                            .AsEnumerable()
                        where usr is not null
                        select ((Func<UserResponse>)(() =>
                        {
                            var item = _mapper.Map<UserResponse>(usr);

                            //_mapper.Map(ext, item.Subscription);

                            return item;
                        }))()).SingleOrDefault();

            return await Result<UserResponse>.SuccessAsync(item);
        }

        public async Task<IResult<MembershipDto>> ChangeMembershipAsync(MembershipDto model)
        {
            try
            {
                var last = await _context.Memberships
                                         .AsNoTracking()
                                         .Where(f => f.UserId.Equals(model.UserId))
                                         .OrderByDescending(o => o.CreatedOn)
                                         .FirstOrDefaultAsync();

                if (last is null)
                {
                    return await Result<MembershipDto>.FailAsync(_localizer["User Not Found!"]);
                }


                if (last.Level == model.Level && last.MaximumAsset == model.MaximumAsset)
                {
                    return await Result<MembershipDto>.FailAsync(string.Format(_localizer["{0} Is Equal"], _localizer["Subscription.Info"]));
                }

                if (DateTime.Now.Subtract(last.CreatedOn).TotalDays < 1)
                {
                    return await Result<MembershipDto>.FailAsync(_localizer["Changes can be made again after 1 day after the change"]);
                }

                var mebership = _mapper.Map<Membership>(model);

                await _intUnitOfWork.Repository<Membership>().AddAsync(mebership);
                await _intUnitOfWork.Commit(new CancellationToken());

                _mapper.Map(mebership, model);

                return await Result<MembershipDto>.SuccessAsync(model, string.Format(_localizer["{0} Updated"], _localizer["Subscriptions"]));
            }
            catch (Exception ex)
            {
                return await Result<MembershipDto>.FailAsync(ex.Message);
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
                return await Result<UserResponse>.FailAsync(_localizer["You are not Authorized."]);
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

                    return await Result<UserResponse>.SuccessAsync(_localizer["User Registered"]);
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

                return await Result<UserResponse>.SuccessAsync(_localizer["User Already Registered"]);
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

        public async Task<int> GetCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
            return count;
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var userSpec = new UserFilterSpecification(searchString);
            var users = await _userManager.Users
                .Specify(userSpec)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
            var result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
                mappers: new Dictionary<string, Func<BlazorHeroUser, object>>
                {
                    { _localizer["Id"], item => item.Id },
                    { _localizer["User.NickName"], item => item.NickName },
                    { _localizer["User.Email"], item => item.Email },
                    { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                    { _localizer["PhoneNumber"], item => item.PhoneNumber },
                    { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                    { _localizer["IsActive"], item => item.IsActive },
                    { _localizer["CreatedOn (Local)"], item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                    //{ _localizer["ProfileImage"], item => item.ProfileImage },
                });

            return result;
        }
    }
}