using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                                                      model.HeadDate.Date <= f.done_at && f.done_at < model.RearDate.Date.AddDays(1))
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

        public async Task<IResult> DailyDeductPoint()
        {
            try
            {
                var users = await _context.Users
                                          .AsNoTracking()
                                          .Include(i => i.Subscription)
                                          .Include(i => i.TradingTerms)
                                          .Include(i => i.Points)
                                          .Where(f => f.Subscription.Level > MembershipLevel.Free && 
                                                      f.Subscription.DailyDeductionPoint > 0)
                                          .ToArrayAsync();

                DateTime now = DateTime.Now;

                Point last = null;
                long balance = 0;

                foreach (var user in users)
                {
                    last = user.Points.SingleOrDefault(f => f.Type == PointType.Deduct &&
                                                            f.done_at.Date == now.Date);

                    if (last is not null ) { continue; }

                    balance = Convert.ToInt64(last?.Balance);

                    if (balance < user.Subscription.DailyDeductionPoint) { continue; }

                    await _unitOfWork.Repository<Point>().AddAsync(new Point()
                    {
                        UserId = user.Id,
                        done_at = now,
                        Membership = user.Subscription.Level,
                        Type = PointType.Deduct,
                        Amount = user.Subscription.DailyDeductionPoint,
                        Balance = balance - user.Subscription.DailyDeductionPoint,
                    });
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}