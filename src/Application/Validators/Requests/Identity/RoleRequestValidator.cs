using Dreamrosia.Koin.Application.Requests.Identity;
using Dreamrosia.Koin.Shared.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Dreamrosia.Koin.Application.Validators.Requests.Identity
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator(IStringLocalizer<SharedLocalizerResources> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required"]);
        }
    }
}
