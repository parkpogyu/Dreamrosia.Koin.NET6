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
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TransferService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public TransferService(IUnitOfWork<string> unitOfWork,
                               BlazorHeroContext context,
                               IMapper mapper,
                               ILogger<TransferService> logger,
                               IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<TransferDto>>> GetTransfersAsync(TransfersRequestDto model)
        {
            try
            {
                var items = await _context.Transfers
                                          .AsNoTracking()
                                          .Where(f => f.UserId.Equals(model.UserId) &&
                                                    model.HeadDate.Date <= f.created_at && f.created_at < model.RearDate.Date.AddDays(1))
                                          .OrderByDescending(o => o.created_at)
                                          .ToArrayAsync();

                return await Result<IEnumerable<TransferDto>>.SuccessAsync(_mapper.Map<IEnumerable<TransferDto>>(items));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<TransferDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<TransferDto>> GetLastTransferAsync(string userId, TransferType type)
        {
            try
            {
                var item = await _context.Transfers
                                         .AsNoTracking()
                                         .Where(f => f.UserId.Equals(userId) && f.type == type)
                                         .OrderByDescending(o => o.created_at)
                                         .FirstOrDefaultAsync();

                return await Result<TransferDto>.SuccessAsync(_mapper.Map<TransferDto>(item));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<TransferDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<int>> SaveTransfersAsync(string userId, IEnumerable<TransferDto> models)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (user is null)
                {
                    return await Result<int>.FailAsync(_localizer["User Not Found!"]);
                }

                var registered = _unitOfWork.Repository<Transfer>()
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

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Transfer>().AddAsync(_mapper.Map<Transfer>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Transfer>().UpdateAsync(old);
                    }
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result<int>.SuccessAsync(models.Count(), string.Format(_localizer["{0} Saved"], _localizer["Transfers"]));
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result<int>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}