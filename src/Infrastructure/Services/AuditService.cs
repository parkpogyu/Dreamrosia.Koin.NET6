using AutoMapper;
using Dreamrosia.Koin.Application.Extensions;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Responses.Audit;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Audit;
using Dreamrosia.Koin.Infrastructure.Specifications;
using Dreamrosia.Koin.Shared.Interfaces.Services;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public AuditService(IMapper mapper,
                            BlazorHeroContext context,
                            IExcelService excelService,
                            IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _mapper = mapper;
            _context = context;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<AuditResponse>>> GetUserAuditTrailsAsync(string userId, DateTime head, DateTime rear)
        {
            var trails = await _context.AuditTrails
                                       .AsNoTracking()
                                       .Where(f => (string.IsNullOrEmpty(userId) ? true : f.UserId == userId) &&
                                                   head.Date <= f.DateTime && f.DateTime < rear.Date.AddDays(1))
                                       .Include(i => i.User)
                                       .OrderByDescending(o => o.DateTime)
                                       .ToArrayAsync();

            return await Result<IEnumerable<AuditResponse>>.SuccessAsync(_mapper.Map<IEnumerable<AuditResponse>>(trails));
        }

        public async Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            var auditSpec = new AuditFilterSpecification(userId, searchString, searchInOldValues, searchInNewValues);
            var trails = await _context.AuditTrails
                .Specify(auditSpec)
                .OrderByDescending(a => a.DateTime)
                .ToListAsync();
            var data = await _excelService.ExportAsync(trails, sheetName: _localizer["Audit trails"],
                mappers: new Dictionary<string, Func<Audit, object>>
                {
                    { _localizer["Table Name"], item => item.TableName },
                    { _localizer["Type"], item => item.Type },
                    { _localizer["Date Time (Local)"], item => DateTime.SpecifyKind(item.DateTime, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["Primary Key"], item => item.PrimaryKey },
                    { _localizer["Old Values"], item => item.OldValues },
                    { _localizer["New Values"], item => item.NewValues },
                });

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}