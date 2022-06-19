using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Client.Infrastructure.Managers;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Settlement
{
    public partial class BankingTransactions
    {
        [Inject] private ISettlementManager SettlmentManager { get; set; }

        private bool _loaded;
        private IEnumerable<BankingTransactionDto> _items { get; set; }
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private readonly BankingTransactionsRequestDto _model = new BankingTransactionsRequestDto();

        protected override async Task OnInitializedAsync()
        {
            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            await GetBankingTransactionsAsync();

            _loaded = true;
        }

        private async Task GetBankingTransactionsAsync()
        {
            _model.HeadDate = Convert.ToDateTime(_dateRange.Start);
            _model.RearDate = Convert.ToDateTime(_dateRange.End);

            var response = await SettlmentManager.GetBankingTransactionsAsync(_model);

            _items = response.Data ?? new List<BankingTransactionDto>();

            if (response.Succeeded) { return; }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        private async Task<IResult<int>> ImportExcel(UploadRequest uploadFile)
        {
            var result = await SettlmentManager.ImportBankingTransactionsAsync(uploadFile);

            return result;
        }

        private async Task InvokeImportModal()
        {
            var parameters = new DialogParameters
            {
                { nameof(ImportExcelModal.ModelName), $"{_localizer["Settlements"]} {_localizer["Transfers"]}"}
            };

            Func<UploadRequest, Task<IResult<int>>> importExcel = ImportExcel;

            parameters.Add(nameof(ImportExcelModal.OnSaved), importExcel);

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };

            var dialog = _dialogService.Show<ImportExcelModal>(_localizer["Import"], parameters, options);

            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await GetBankingTransactionsAsync();
            }
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            _dateRangeTerm = value;

            await GetBankingTransactionsAsync();
        }
    }
}

