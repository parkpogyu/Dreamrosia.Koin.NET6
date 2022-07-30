using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Helpers;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Role;
using Dreamrosia.Koin.Shared.Constants.User;
using Dreamrosia.Koin.Shared.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;
        private readonly BlazorHeroContext _db;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public DatabaseSeeder(UserManager<BlazorHeroUser> userManager,
                              RoleManager<BlazorHeroRole> roleManager,
                              BlazorHeroContext db,
                              ILogger<DatabaseSeeder> logger,
                              IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }

        public void Initialize()
        {
            AddRoles();

            AddAdministrator();

            _db.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if User Exists
                var superUser = new BlazorHeroUser
                {
                    Email = "mjtobi@gmail.com",
                    UserName = "mjtobi@gmail.com",
                    NickName = "박보규",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };

                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);

                if (superUserInDb == null)
                {
                    var createResult = await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var rollResult = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    var externalResult = await _userManager.AddLoginAsync(superUser, new UserLoginInfo("KakaoTalk", "2018007078", "KakaoTalk"));

                    //_ = _askTermsService.AddAskTermsAsync(superUser.Id);
                    //_ = _bidTermsService.AddBidTermsAsync(superUser.Id);

                    await _db.Database.ExecuteSqlRawAsync($"CALL PRC_Initialize_User_Terms('{superUser.Id}')");

                    if (createResult.Succeeded && rollResult.Succeeded && externalResult.Succeeded)
                    {
                        _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        var errors = new List<IdentityError>();

                        errors.AddRange(createResult.Errors);
                        errors.AddRange(rollResult.Errors);
                        errors.AddRange(externalResult.Errors);

                        foreach (var error in errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private void AddRoles()
        {
            Task.Run(async () =>
            {
                #region Admin
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);

                if (adminRoleInDb == null)
                {
                    var adminRole = new BlazorHeroRole(RoleConstants.AdministratorRole, _localizer["Administrator role with full permissions"]);

                    await _roleManager.CreateAsync(adminRole);

                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);

                    _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                }

                foreach (var permission in Permissions.GetRegisteredPermissions())
                {
                    await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                }
                #endregion

                List<string> roles = new List<string>()
                {
                    Permissions.Dashboards.View,
                    Permissions.Users.View,
                    Permissions.Symbols.View,
                    Permissions.Symbols.Export,
                    Permissions.Candles.View,
                    Permissions.Candles.Export,
                    Permissions.TradingTerms.View,
                    Permissions.TradingTerms.Edit,
                    Permissions.UPbitKeys.View,
                    Permissions.UPbitKeys.Edit,
                    Permissions.Assets.View,
                    Permissions.Assets.Export,
                    Permissions.Orders.View,
                    Permissions.Orders.Export,
                    Permissions.Positions.View,
                    Permissions.Positions.Export,
                    Permissions.Transfers.View,
                    Permissions.Transfers.Export,
                    Permissions.BackTestings.BackTesting,
                };

                #region Basic
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);

                if (basicRoleInDb == null)
                {
                    var basicRole = new BlazorHeroRole(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);

                    await _roleManager.CreateAsync(basicRole);

                    basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);

                    _logger.LogInformation(_localizer["Seeded Basic Role."]);
                }

                foreach (var permission in Permissions.GetRegisteredPermissions()
                                                      .Where(f => roles.Contains(f)))
                {
                    await _roleManager.AddPermissionClaim(basicRoleInDb, permission);
                }
                #endregion

                #region Partner
                var partnerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.PartnerRole);

                if (partnerRoleInDb == null)
                {
                    var partnerRole = new BlazorHeroRole(RoleConstants.PartnerRole, _localizer["Partner role with default permissions"]);

                    await _roleManager.CreateAsync(partnerRole);

                    partnerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.PartnerRole);

                    _logger.LogInformation(_localizer["Seeded Partner Role."]);
                }

                foreach (var permission in Permissions.GetRegisteredPermissions()
                                                      .Where(f => roles.Contains(f)))
                {
                    await _roleManager.AddPermissionClaim(partnerRoleInDb, permission);
                }
                #endregion

            }).GetAwaiter().GetResult();
        }
    }
}