using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Dreamrosia.Koin.Application.Validators.Requests.Identity
{
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<SharedLocalizerResources> localizer)
        {
            RuleFor(request => request.NickName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Nick Name is required"]);
        }
    }
}