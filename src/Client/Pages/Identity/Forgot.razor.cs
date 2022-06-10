using Blazored.FluentValidation;
using Dreamrosia.Koin.Application.Requests.Identity;
using MudBlazor;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Identity
{
    public partial class Forgot
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly ForgotPasswordRequest _emailModel = new();

        private async Task SubmitAsync()
        {
            var result = await _accountManager.ForgotPasswordAsync(_emailModel);

            if (result.Succeeded)
            {
                _snackBar.Add(_localizer["Done!"], Severity.Success);
                _navigationManager.NavigateTo("/");
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}