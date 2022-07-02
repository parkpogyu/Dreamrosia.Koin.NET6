using AutoMapper;
using Blazored.FluentValidation;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Enums;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Personal
{
    public partial class UPbitKey
    {
        [Inject] private IUPbitKeyManager UPbitKeyManager { get; set; }
        [Parameter] public PageModes PageMode { get; set; }
        [Parameter] public string UserId { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private bool _loaded;
        public string _userId { get; private set; }
        private readonly UPbitKeyDto _model = new();
        private readonly UPbitKeyTestDto _testModel = new();
        private bool _access_keyVisibility;
        private InputType _access_keyInput = InputType.Password;
        private string _access_keyInputIcon = Icons.Material.Filled.VisibilityOff;
        private bool _secret_keyVisibility;
        private InputType _secret_keyInput = InputType.Password;
        private string _secret_keyInputIcon = Icons.Material.Filled.VisibilityOff;
        private string _allowedIPs { get; set; }

        protected override async Task OnInitializedAsync()
        {
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

            _mapper = new MapperConfiguration(c => { c.AddProfile<UPbitKeyProfile>(); }).CreateMapper();

            await GetUPbitKeyAsync();

            _loaded = true;
        }

        private async Task GetUPbitKeyAsync()
        {
            var response = await UPbitKeyManager.GetUPbitKeyAsync(_userId);

            _mapper.Map(response.Data ?? new UPbitKeyDto(), _model);

            if (!response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            var ips = await UPbitKeyManager.GetAllowedIPsAsync();

            if (!ips.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _allowedIPs = string.Join(",", ips.Data);
        }

        private async Task UpdateUPbitKeyAsync()
        {
            if (_model.access_key.Equals(_testModel.access_key) &&
                _model.secret_key.Equals(_testModel.secret_key) && _testModel.IsAuthenticated)
            {
                var response = await UPbitKeyManager.UpdateUPbitKeyAsync(_model);

                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, response.Succeeded ? Severity.Success : Severity.Error);
                }
            }
        }

        private async Task TestUPbitKeyAsync()
        {
            var request = _mapper.Map<UPbitKeyTestDto>(_model);

            if (string.IsNullOrEmpty(request.UserId))
            {
                request.UserId = _userId;
            }

            var response = await UPbitKeyManager.TestUPbitKeyAsync(request);

            _mapper.Map(response.Data ?? new UPbitKeyTestDto(), _testModel);
            _mapper.Map(response.Data ?? new UPbitKeyTestDto(), _model);

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, response.Succeeded ? Severity.Success : Severity.Error);
            }
        }

        private void ToggleAccessKeyVisibility()
        {
            if (_access_keyVisibility)
            {
                _access_keyVisibility = false;
                _access_keyInputIcon = Icons.Material.Filled.VisibilityOff;
                _access_keyInput = InputType.Password;
            }
            else
            {
                _access_keyVisibility = true;
                _access_keyInputIcon = Icons.Material.Filled.Visibility;
                _access_keyInput = InputType.Text;
            }
        }

        private void ToggleSecretKeyVisibility()
        {
            if (_secret_keyVisibility)
            {
                _secret_keyVisibility = false;
                _secret_keyInputIcon = Icons.Material.Filled.VisibilityOff;
                _secret_keyInput = InputType.Password;
            }
            else
            {
                _secret_keyVisibility = true;
                _secret_keyInputIcon = Icons.Material.Filled.Visibility;
                _secret_keyInput = InputType.Text;
            }
        }

        private void DisableUPbitKeySaveButton()
        {
            _testModel.IsPassed = false;
        }
    }
}