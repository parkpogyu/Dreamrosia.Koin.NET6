using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Interfaces.Services.Identity;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Infrastructure.Specifications;
using Dreamrosia.Koin.Shared.Constants.Role;
using Dreamrosia.Koin.Shared.Interfaces.Services;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly SignInManager<BlazorHeroUser> _signInManager;
        private readonly BlazorHeroContext _context;
        private readonly IUnitOfWork<string> _strUnitOfWork;
        private readonly IUnitOfWork<int> _intUnitOfWork;
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExcelService _excelService;
        private readonly IUploadService _uploadService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public UserService(UserManager<BlazorHeroUser> userManager,
                           SignInManager<BlazorHeroUser> signInManager,
                           BlazorHeroContext context,
                           IUnitOfWork<string> strUnitOfWork,
                           IUnitOfWork<int> intUnitOfWork,
                           IAccountService accountService,
                           ICurrentUserService currentUserService,
                           IExcelService excelService,
                           IUploadService uploadService,
                           IMapper mapper,
                           ILogger<UserService> logger,
                           IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _strUnitOfWork = strUnitOfWork;
            _intUnitOfWork = intUnitOfWork;
            _accountService = accountService;
            _currentUserService = currentUserService;
            _excelService = excelService;
            _uploadService = uploadService;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<UserDto>> GetAsync(string userId)
        {
            try
            {
                var item = await _context.Users
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (item is null)
                {
                    return await Result<UserDto>.FailAsync(_localizer["User Not Found!"]);
                }

                return await Result<UserDto>.SuccessAsync(_mapper.Map<UserDto>(item));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UserDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<SubscriptionDto>> GetSubscriptionAsync(string userId)
        {
            try
            {
                var user = (from usr in _context.Users
                                                .AsNoTracking()
                                                .Where(f => f.Id.Equals(userId))
                                                .Include(i => i.Subscription).ThenInclude(i => i.Recommender)
                                                .Include(i => i.Memberships)
                                                .Include(i => i.MiningBotTicket)
                                                .Include(i => i.TradingTerms)
                                                .AsEnumerable()
                            from usrcode in _context.UserLogins
                                                    .AsNoTracking()
                                                    .Where(f => f.UserId.Equals(usr.Id))
                                                    .AsEnumerable()
                            from reccode in _context.UserLogins
                                                    .AsNoTracking()
                                                    .Where(f => f.UserId.Equals(usr.Subscription.RecommenderId)).DefaultIfEmpty()
                                                    .AsEnumerable()
                            select ((Func<SubscriptionDto>)(() =>
                            {
                                var item = _mapper.Map<SubscriptionDto>(usr);

                                item.UserCode = usrcode.ProviderKey;
                                item.AutoTrading = usr.TradingTerms.AutoTrading;
                                item.TimeFrame = usr.TradingTerms.TimeFrame;
                                item.IsAssignedBot = usr.MiningBotTicket is null ? false : true;

                                var membership = usr.Memberships.OrderByDescending(o => o.CreatedOn).First();

                                item.Membership = _mapper.Map<MembershipDto>(membership);

                                item.Recommender = _mapper.Map<UserDto>(usr.Subscription.Recommender);

                                if (item.Recommender is not null)
                                {
                                    item.Recommender.UserCode = reccode?.ProviderKey;
                                }

                                return item;
                            }))()).SingleOrDefault();

                if (user is null)
                {
                    return await Result<SubscriptionDto>.FailAsync(_localizer["User Not Found!"]);
                }

                return await Result<SubscriptionDto>.SuccessAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<SubscriptionDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        private async Task<IEnumerable<UserSummaryDto>> GetUsersAsync(DateTime head, DateTime rear, string userId = null, bool? goBoast = null)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);

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
                             item.MaximumAsset = isAdmin ? membership.MaximumAsset : 0;
                             item.DailyDeductionPoint = isAdmin ? membership.DailyDeductionPoint : 0;

                             item.AutoTrading = usr.TradingTerms.AutoTrading;
                             item.TimeFrame = usr.TradingTerms.TimeFrame;
                             item.IsAssignedBot = usr.MiningBotTicket is null ? false : true;

                             return item;
                         }))()).ToArray();

            return items;
        }

        public async Task<IResult<IEnumerable<UserSummaryDto>>> GetSummariesAsync(DateTime head, DateTime rear)
        {
            try
            {
                var items = await GetUsersAsync(head, rear);

                var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);

                var isAdmin = await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole);

                if (isAdmin)
                {
                    foreach (var item in items)
                    {
                        var roles = await _accountService.GetRolesAsync(item.Id);

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
                var items = await GetUsersAsync(head, rear, userId);

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
                var items = await GetUsersAsync(head, rear, null, true);

                return await Result<IEnumerable<UserSummaryDto>>.SuccessAsync(items);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<UserSummaryDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<UserDto>> GetRecommenderAsync(RecommenderDto model)
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
                             select ((Func<UserDto>)(() =>
                             {
                                 var item = _mapper.Map<UserDto>(rec);

                                 return item;
                             }))()).SingleOrDefault();

                return await Result<UserDto>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UserDto>.FailAsync(_localizer["An unhandled error has occurred."]);
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

        public async Task<IResult<UserDto>> GetAccountHolderAsync(string userCode)
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
                        select ((Func<UserDto>)(() =>
                        {
                            var item = _mapper.Map<UserDto>(usr);

                            //_mapper.Map(ext, item.Subscription);

                            return item;
                        }))()).SingleOrDefault();

            return await Result<UserDto>.SuccessAsync(item);
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

        public async Task<int> GetCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
            return count;
        }

        public async Task<IResult<UserDto>> GetProfileAsync(string userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

            var result = _mapper.Map<UserDto>(user);

            return await Result<UserDto>.SuccessAsync(result);
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