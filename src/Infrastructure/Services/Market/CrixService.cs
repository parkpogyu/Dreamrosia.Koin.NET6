using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
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
    public class CrixService : ICrixService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CrixService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public CrixService(IUnitOfWork<string> unitOfWork,
                           BlazorHeroContext context,
                           IMapper mapper,
                           ILogger<CrixService> logger,
                           IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<CrixDto>>> GetCrixesAsync()
        {
            try
            {
                var items = await _context.Crixes.ToArrayAsync();

                return await Result<IEnumerable<CrixDto>>.SuccessAsync(_mapper.Map<IEnumerable<CrixDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<CrixDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SaveCrixesAsync(IEnumerable<CrixDto> models)
        {
            try
            {
                var entities = _unitOfWork.Repository<Crix>().Entities;

                var items = (from lt in models
                             from rt in entities.Where(f => f.Id.Equals(lt.crix_code)).DefaultIfEmpty()
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Crix>().AddAsync(_mapper.Map<Crix>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Crix>().UpdateAsync(old);
                    }
                }

                var removes = (from lt in entities.AsEnumerable()
                               from rt in models.Where(f => f.crix_code.Equals(lt.Id)).DefaultIfEmpty()
                               where rt == null
                               select lt).ToArray();

                foreach (var remove in removes)
                {
                    await _unitOfWork.Repository<Crix>().DeleteAsync(remove);
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Crixes"]));

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