using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Enums;
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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public OrderService(IUnitOfWork<string> unitOfWork,
                            BlazorHeroContext context,
                            IMapper mapper,
                            ILogger<OrderService> logger,
                            IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<OrderDto>>> GetOrdersAsync(OrdersRequestDto model)
        {
            try
            {
                var items = await _context.Orders
                                          .AsNoTracking()
                                          .Where(f => f.UserId.Equals(model.UserId) &&
                                                      model.HeadDate.Date <= f.created_at && f.created_at < model.RearDate.Date.AddDays(1))
                                          .OrderByDescending(o => o.created_at)
                                          .ThenBy(o => o.side)
                                          .ToArrayAsync();

                return await Result<IEnumerable<OrderDto>>.SuccessAsync(_mapper.Map<IEnumerable<OrderDto>>(items));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<OrderDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<OrderDto>> GetLastOrderAsync(string userId)
        {
            try
            {
                var item = await _context.Orders
                                         .AsNoTracking()
                                         .Where(f => f.UserId.Equals(userId))
                                         .OrderByDescending(o => o.created_at)
                                         .FirstOrDefaultAsync();

                return await Result<OrderDto>.SuccessAsync(_mapper.Map<OrderDto>(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<OrderDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SaveOrdersAsync(string userId, IEnumerable<OrderDto> models, bool done)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (user is null)
                {
                    return await Result.FailAsync(_localizer["User Not Found!"]);
                }

                var registered = _unitOfWork.Repository<Order>()
                                            .Entities
                                            .Where(f => f.UserId.Equals(userId));

                var items = (from lt in models
                             from rt in registered.Where(f => f.Id.Equals(lt.uuid)).DefaultIfEmpty()
                             orderby lt.created_at
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    neo.UserId = userId;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Order>().AddAsync(_mapper.Map<Order>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Order>().UpdateAsync(old);
                    }
                }

                if (!done)
                {
                    var removes = (from lt in registered.Where(f => f.state == OrderState.wait ||
                                                                    f.state == OrderState.watch).AsEnumerable()
                                   from rt in models.Where(f => f.uuid.Equals(lt.Id)).DefaultIfEmpty()
                                   where rt == null
                                   select lt).ToArray();

                    foreach (var remove in removes)
                    {
                        await _unitOfWork.Repository<Order>().DeleteAsync(remove);
                    }
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Saved"], _localizer["Orders"]));
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