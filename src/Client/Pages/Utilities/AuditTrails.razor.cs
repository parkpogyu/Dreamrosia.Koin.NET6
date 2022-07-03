using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Responses.Audit;
using Dreamrosia.Koin.Client.Extensions;
using Dreamrosia.Koin.Client.Infrastructure.Managers.Audit;
using Dreamrosia.Koin.Client.Shared.Components;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Constants.Permission;
using Dreamrosia.Koin.Shared.Constants.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Pages.Utilities
{
    public partial class AuditTrails 
    {
        [Inject] private IAuditManager AuditManager { get; set; }
        [Parameter] public string UserId { get; set; }

        private bool _loaded;
        private MudTable<RelatedAuditTrail> _table;
        private string _userId { get; set; }
        public IEnumerable<RelatedAuditTrail> _items = new List<RelatedAuditTrail>();
        private string _searchString = "";
        private bool _chkIsAllUser = true;
        private bool _searchInOldValues = false;
        private bool _searchInNewValues = false;
        private DateRange _dateRange { get; set; } = new DateRange();
        private DateRangeTerms _dateRangeTerm { get; set; } = DateRangeTerms._1W;
        private bool _canExportAuditTrails;

        private Guid _resizeSubscribedId { get; set; }
        private bool _isDivTableRendered { get; set; } = false;
        private string _divTableHeight { get; set; } = "100%";
        private readonly string _divTableId = Guid.NewGuid().ToString();

        protected override async Task OnInitializedAsync()
        {
            var now = DateTime.Now.Date;

            _dateRange.Start = now.GetBefore(_dateRangeTerm);
            _dateRange.End = now;

            var user = _authenticationManager.CurrentUser();

            _canExportAuditTrails = (await _authorizationService.AuthorizeAsync(user, Permissions.AuditTrails.Export)).Succeeded;

            if (string.IsNullOrEmpty(UserId))
            {
                _userId = user.GetUserId();
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

            await GetAuditTrailsAsync();

            _loaded = true;
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

        private async Task GetAuditTrailsAsync()
        {
            var response = await AuditManager.GetUserAuditTrailsAsync(_chkIsAllUser ? null : _userId,
                                                                      _dateRange.Start,
                                                                      _dateRange.End);

            if (response.Succeeded)
            {
                _items = response.Data.Select(x => new RelatedAuditTrail
                {
                    AffectedColumns = x.AffectedColumns,
                    DateTime = x.DateTime,
                    Id = x.Id,
                    NewValues = x.NewValues,
                    OldValues = x.OldValues,
                    PrimaryKey = x.PrimaryKey,
                    TableName = x.TableName,
                    Type = x.Type,
                    UserId = x.UserId,
                    User = x.User,
                    LocalTime = DateTime.SpecifyKind(x.DateTime, DateTimeKind.Utc).ToLocalTime()
                }).ToList();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task CheckAllUserChanged(bool value)
        {
            _chkIsAllUser = value;

            await GetAuditTrailsAsync();
        }

        private async Task SelectedTermChanged(DateRangeTerms value)
        {
            if (_dateRangeTerm == value) { return; }

            _dateRangeTerm = value;

            await GetAuditTrailsAsync();
        }

        private void RowClickEvent(TableRowClickEventArgs<RelatedAuditTrail> args)
        {
            if (args.Item is null) { return; }

            var audit = _items.Single(f => f.Id.Equals(args.Item.Id));

            foreach (var item in _items.Where(f => !f.Id.Equals(audit.Id)))
            {
                item.ShowDetails = false;
            }

            audit.ShowDetails = !audit.ShowDetails;
        }

        private bool Search(AuditResponse response)
        {
            var result = false;

            // check Search String
            if (string.IsNullOrWhiteSpace(_searchString)) result = true;

            if (!result)
            {
                if (response.TableName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
                if (_searchInOldValues &&
                    response.OldValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
                if (_searchInNewValues &&
                    response.NewValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
            }

            return result || response.User.NickName.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
        }

        private async Task ExportToExcelAsync()
        {
            var response = await AuditManager.DownloadFileAsync(_searchString, _searchInOldValues, _searchInNewValues);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(AuditTrails).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Audit Trails exported"]
                    : _localizer["Filtered Audit Trails exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        public async ValueTask DisposeAsync() => await _resizeService.Unsubscribe(_resizeSubscribedId);


        public class RelatedAuditTrail : AuditResponse
        {
            public bool ShowDetails { get; set; } = false;
            public DateTime LocalTime { get; set; }
        }
    }
}