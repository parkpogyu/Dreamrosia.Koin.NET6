using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Dreamrosia.Koin.Application.Validators.Requests.Identity
{
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IStringLocalizer<SharedLocalizerResources> localizer)
        {
            RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"])
                .EmailAddress().WithMessage(x => localizer["Email is not correct"]);
        }
    }
}