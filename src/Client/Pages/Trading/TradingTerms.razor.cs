using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Mappings;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Trading
{
    public partial class TradingTerms
    {
        [Inject] private ITradingTermsManager TradingTermsManager { get; set; }
        [Inject] private IMarketManager MarketManager { get; set; }

        [Parameter] public string UserId { get; set; }

        private IMapper _mapper;

        private bool _loaded;
        private string _userId { get; set; }

        private BackTestingRequestDto _model { get; set; }
        private IEnumerable<SymbolDto> _symbols { get; set; } = new List<SymbolDto>();

        protected override async Task OnInitializedAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<TradingTermsProfile>(); }).CreateMapper();

            if (string.IsNullOrEmpty(UserId))
            {
                var user = await _authenticationManager.CurrentUser();

                _userId = user.GetUserId();
            }
            else
            {
                var isAdmin = _stateProvider.IsAdministrator();

                if (!isAdmin)
                {
                    _snackBar.Add(_localizer["You are not Authorized."], Severity.Error);
                    _navigationManager.NavigateTo("/");
                    return;
                }

                _userId = UserId;
            }

            List<Task> tasks = new List<Task>();

            tasks.Add(GetTradingTermsAsync());
            tasks.Add(GetSymbolsAsync());

            await Task.WhenAll(tasks).ConfigureAwait(false);

            _loaded = true;
        }

        private async Task GetTradingTermsAsync()
        {
            var response = await TradingTermsManager.GetTradingTermsAsync(_userId);

            _model = _mapper.Map<BackTestingRequestDto>(response.Data ?? new BackTestingRequestDto());

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task GetSymbolsAsync()
        {
            var response = await MarketManager.GetSymbolsAsync();

            _symbols = response.Data ?? new List<SymbolDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}