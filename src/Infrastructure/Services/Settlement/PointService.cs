using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class PointService : IPointService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PointService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public PointService(IUnitOfWork<int> unitOfWork,
                            BlazorHeroContext context,
                            IMapper mapper,
                            ILogger<PointService> logger,
                            IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<PointDto>>> GetPointsAsync(PointsRequestDto model)
        {
            try
            {
                var items = await _context.Points
                                          .AsNoTracking()
                                          .Include(i => i.Transaction)
                                          .Where(f => f.UserId.Equals(model.UserId) &&
                                                      f.done_at >= model.HeadDate.Date &&
                                                      f.done_at <= model.RearDate.Date.AddDays(1).AddSeconds(-1))
                                          .OrderByDescending(o => o.done_at)
                                          .ToArrayAsync();

                return await Result<IEnumerable<PointDto>>.SuccessAsync(_mapper.Map<IEnumerable<PointDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<PointDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}