using Blazored.FluentValidation;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Responses.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Personal
{
    public partial class RecommenderModal
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly RecommenderDto _model = new();
        private UserResponse _recommender { get; set; } = new UserResponse();
        [Parameter] public string UserId { get; set; }
        [Parameter] public string UserCode { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _model.UserId = UserId;
            _model.UserCode = UserCode;

            await GetRecommenderAsync();
        }

        private async Task GetRecommenderAsync()
        {
            if (string.IsNullOrEmpty(_model.UserCode))
            {
                _snackBar.Add(string.Format(_localizer["{0} Not Found"], _localizer["Input"]), Severity.Error);

                return;
            }

            var response = await _userManager.GetRecommenderAsync(_model);

            if (response.Succeeded)
            {
                _recommender = response.Data is null ? new UserResponse() : response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task UpdateRecommenderAsync()
        {
            var model = new RecommenderDto()
            {
                UserId = _model.UserId,
                RecommenderId = _recommender.Id,
            };

            var response = await _userManager.UpdateRecommenderAsync(model);

            if (response.Succeeded)
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Success);
                }

                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}