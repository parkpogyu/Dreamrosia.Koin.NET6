using AutoMapper;
using Blazored.FluentValidation;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Shared.Dialogs;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Personal
{
    public partial class Subscription
    {
        [Parameter] public string UserId { get; set; }

        private FluentValidationValidator _membershipValidator;
        private bool _membershipValidated => _membershipValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private bool _loaded;
        private string _userId { get; set; }
        private SubscriptionDto _user = new();
        private MembershipDto _model { get; set; }

        private async Task ToggleUserStatus()
        {
            var request = new ToggleUserStatusRequest { ActivateUser = _user.IsActive, UserId = UserId };

            var result = await _accountManager.ToggleUserStatusAsync(request);

            if (result.Succeeded)
            {
                _snackBar.Add(_localizer["Updated User Status."], Severity.Success);

                _navigationManager.NavigateTo("/identity/users");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c =>
            {
                c.AddProfile<MembershipProfile>();
            }).CreateMapper();

            if (string.IsNullOrEmpty(UserId))
            {
                _userId = _authenticationManager.CurrentUser().GetUserId();
            }
            else
            {
                if (!_stateProvider.IsInRole(RoleConstants.AdministratorRole))
                {
                    _snackBar.Add(_localizer["You are not Authorized."], Severity.Error);
                    _navigationManager.NavigateTo("/");
                    return;
                }

                _userId = UserId;
            }

            await GetSubscriptionAsync();

            _loaded = true;
        }

        private async Task GetSubscriptionAsync()
        {
            var result = await _userManager.GetSubscriptionAsync(_userId);

            _user = result.Data ?? new SubscriptionDto();
            _model = _mapper.Map<MembershipDto>(_user.Membership ?? new MembershipDto());

            if (result.Succeeded) { return; }

            foreach (var error in result.Messages)
            {
                _snackBar.Add(error, Severity.Error);
            }
        }

        private async Task InvokeRecommenderModal()
        {
            var parameters = new DialogParameters();

            parameters.Add("UserId", _userId);
            parameters.Add("UserCode", _user.Recommender?.UserCode);

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };

            var dialog = _dialogService.Show<RecommenderModal>(_localizer["Change"], parameters, options);

            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            await GetSubscriptionAsync();
        }

        private void MembershipLevelChanged(MembershipLevel value)
        {
            _model.Level = value;

            if (_model.Level == MembershipLevel.Free)
            {
                _model.DailyDeductionPoint = DefaultValue.ChargingPoint.Free;
                _model.MaximumAsset = DefaultValue.TradingTerms.MaximumAsset4Free;
            }
            else if (_model.Level == MembershipLevel.Basic)
            {
                _model.DailyDeductionPoint = DefaultValue.ChargingPoint.Basic;
                _model.MaximumAsset = DefaultValue.TradingTerms.MaximumAsset4Basic;
            }
            else
            {
                if (_model.MaximumAsset < DefaultValue.TradingTerms.MinimumAsset4Advanced)
                {
                    _model.MaximumAsset = DefaultValue.TradingTerms.MinimumAsset4Advanced;
                }

                SetDailyDeductionPoint();
            }

            StateHasChanged();
        }

        private void MaximumAssetChanged(float value, bool update = true)
        {
            var solution = value / DefaultValue.ChargingPoint.Divider;

            var integer = (long)solution;

            var real = solution - integer;

            if (real == 0)
            {
                _model.MaximumAsset = value;
            }
            else if (real < .5)
            {
                _model.MaximumAsset = integer * DefaultValue.ChargingPoint.Divider;
            }
            else
            {
                _model.MaximumAsset = (integer + 1) * DefaultValue.ChargingPoint.Divider;
            }

            SetDailyDeductionPoint();

            if (update)
            {
                StateHasChanged();
            }
        }

        private void SetDailyDeductionPoint()
        {
            _model.DailyDeductionPoint = _model.GetDailyDeductionPoint();
        }

        private async Task ChangeMembershipAsync()
        {
            var parameters = new DialogParameters();

            parameters.Add("ContentText", $"{_localizer["Do you want to change membership level related information?"]}");

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };

            var dialog = _dialogService.Show<Confirmation>($"{_localizer["Subscriptions"]} {_localizer["Change"]}", parameters, options);

            var result = await dialog.Result;

            if (result.Cancelled) { return; }

            var response = await _userManager.ChangeMembershipAsync(_model);

            if (response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Success);

                    _model = _mapper.Map<MembershipDto>(response.Data);

                    //_mapper.Map(response.Data, _user.Subscription.Membership);
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}