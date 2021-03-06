using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Shared.Components
{
    public partial class BankingTransactionTable : IAsyncDisposable
    {
        [CascadingParameter(Name = "BankingTransactions")]
        private IEnumerable<BankingTransactionDto> BankingTransactions
        {
            get => _sources;
            set
            {
                _sources = value;

                SetItems();
            }
        }
        [Parameter] public bool IsUserView { get; set; } = false;

        private IEnumerable<BankingTransactionDto> _sources { get; set; }
        private IEnumerable<BankingTransactionDto> _items { get; set; }
        private BankingTransactionDto _selectedItem { get; set; }
        private string _selectedTransferType { get; set; }
        private IEnumerable<string> _selectedTransferTypes { get; set; }
        private string _searchString { get; set; } = string.Empty;
        private bool _canEditBankingTransaction { get; set; } = false;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnInitializedAsync()
        {
            var user = _authenticationManager.CurrentUser();

            _canEditBankingTransaction = (await _authorizationService.AuthorizeAsync(user, Permissions.BankingTransactions.Edit)).Succeeded;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    _resizeSubscribedId = await _resizeService.Subscribe((size) =>
                    {
                        if (!_isDivTableRendered) { return; }

                        InvokeAsync(SetDivHeightAsync);

                    }, new ResizeOptions
                    {
                        NotifyOnBreakpointOnly = false,
                    });
                }
                else
                {
                    if (_isDivTableRendered) { return; }

                    var isRendered = await _jsRuntime.InvokeAsync<bool>("func_isRendered", _divTableId);

                    if (!isRendered) { return; }

                    _isDivTableRendered = isRendered;

                    await SetDivHeightAsync();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                await base.OnAfterRenderAsync(firstRender);
            }
        }

        private async Task SetDivHeightAsync()
        {
            var window = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_getWindowSize");
            var rect = await _jsRuntime.InvokeAsync<BoundingClientRect>("func_BoundingClientRect.get", _divTableId);

            if (rect is null) { return; }

            if (BoundingClientRect.IsMatchMediumBreakPoints(window.Height))
            {
                var divHeight = (window.Height - rect.Top - 62 - 52 - 8);

                _divTableHeight = $"{divHeight}px";
            }
            else
            {
                _divTableHeight = "auto";
            }

            StateHasChanged();
        }

        private void SetItems()
        {
            _items = _sources.Where(f => (_selectedTransferTypes is null ? true :
                                          _selectedTransferTypes.Any() ? _selectedTransferTypes.Contains(f.TransferType.ToDescriptionString()) : true)).ToArray();

        }

        private void TransferTypeSelectionChanged(IEnumerable<string> values)
        {
            _selectedTransferTypes = values;

            SetItems();
        }

        private string GetTransferTypeSelectionText(List<string> values)
        {
            var count = values.Count();

            if (count == 0)
            {
                return string.Empty;
            }
            else if (count == 1)
            {
                return $"{values.First()}";
            }
            else
            {
                return $"{values.First()}, ...";
            }
        }

        private bool Search(BankingTransactionDto item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;

            return (item.Contents?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Counterparty?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.Memo?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.UserCode?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.AccountHolder?.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true ||
                    item.AccountHolder?.NickName.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true) ? true : false;
        }

        private async Task BankingTransactionsToExcelAsync()
        {
            try
            {
                var data = await _excelService.ExportAsync(_sources,
                    sheetName: _localizer["BankingTransactions"],
                    mappers: new Dictionary<string, Func<BankingTransactionDto, object>>
                    {
                        { _localizer["BankingTransaction.done_at"] ,item=>item.done_at.Date },
                        { _localizer["BankingTransaction.Classification"] ,item=>item.Classification},
                        { _localizer["BankingTransaction.Contents"] ,item=>item.Contents },
                        { _localizer["BankingTransaction.Deposit"] ,item=>item.Deposit },
                        { _localizer["BankingTransaction.Withdraw"] ,item=>item.Withdraw },
                        { _localizer["BankingTransaction.Balance"] ,item=>item.Balance },
                        { _localizer["BankingTransaction.Counterparty"] ,item=>item.Counterparty },
                        { _localizer["BankingTransaction.Memo"] ,item=>item.Memo },
                        { _localizer["Subscription.UserCode"] ,item=>item.UserCode },
                    });

                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = data,
                    FileName = $"{_localizer["Settlement.BankingTransactions"]}.xlsx",

                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
            }
            catch (Exception)
            {
            }
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);
    }
}
