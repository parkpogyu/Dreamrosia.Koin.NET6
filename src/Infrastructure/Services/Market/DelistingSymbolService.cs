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
    public class DelistingSymbolService : IDelistingSymbolService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly IMapper _mapper;
        private readonly ILogger<SymbolService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public DelistingSymbolService(IUnitOfWork<string> unitOfWork,
                                      BlazorHeroContext context,
                                      IUPbitTickerService upbitTickerService,
                                      IMapper mapper,
                                      ILogger<SymbolService> logger,
                                      IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _upbitTickerService = upbitTickerService;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<DelistingSymbolDto>>> GetDelistingSymbolsAsync()
        {
            try
            {
                var items = await _context.DelistingSymbols
                                          .AsNoTracking()
                                          .OrderByDescending(f => f.CloseAt)
                                          .ToArrayAsync();

                return await Result<IEnumerable<DelistingSymbolDto>>.SuccessAsync(_mapper.Map<IEnumerable<DelistingSymbolDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<DelistingSymbolDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> RegistDelistingSymbolAsync(DelistingSymbolDto model)
        {
            try
            {
                var item = _unitOfWork.Repository<DelistingSymbol>().Entities.SingleOrDefault(f => f.Id.Equals(model.market));

                if (item is null)
                {
                    await _unitOfWork.Repository<DelistingSymbol>().AddAsync(_mapper.Map<DelistingSymbol>(model));
                }
                else
                {
                    await _unitOfWork.Repository<DelistingSymbol>().UpdateAsync(_mapper.Map<DelistingSymbol>(model));
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result<string>.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Delisting Symbols"]));
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> DeleteDelistingSymbolsAsync(IEnumerable<DelistingSymbolDto> models)
        {
            try
            {
                var items = (from lt in models
                             from rt in _context.DelistingSymbols.AsEnumerable().Where(f => f.Id.Equals(lt.market)).DefaultIfEmpty()
                             where rt is not null
                             select rt).ToArray();

                foreach (var item in items)
                {
                    await _unitOfWork.Repository<DelistingSymbol>().DeleteAsync(item);
                }

                if (items.Any())
                {
                    await _unitOfWork.Commit(new CancellationToken());

                    return await Result<string>.SuccessAsync(string.Format(_localizer["{0} Deleted"], _localizer["Delisting Symbols"]));
                }
                else
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["Delisting Symbols"]));
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> CollectDelistingSymbolsAsync()
        {
            try
            {
                var response = await _upbitTickerService.GetDelistingSymbolsAsync();

                if (response.Succeeded)
                {
                    var registered = _context.DelistingSymbols.AsNoTracking().ToArray();
                    var symbols = _context.Symbols.AsNoTracking().ToArray();

                    var items = (from lt in response.Data
                                 from rt in registered.Where(f => f.Id.Equals(lt.market)).DefaultIfEmpty()
                                 from symbol in symbols.Where(f => f.Id.Equals(lt.market)).DefaultIfEmpty()
                                 where rt is null
                                 select ((Func<DelistingSymbolDto>)(() =>
                                 {
                                     lt.korean_name = symbol?.korean_name;

                                     return lt;
                                 }))()).ToArray();

                    foreach (var item in items)
                    {
                        await _unitOfWork.Repository<DelistingSymbol>().AddAsync(_mapper.Map<DelistingSymbol>(item));
                    }

                    if (items.Any())
                    {
                        await _unitOfWork.Commit(new CancellationToken());

                        return await Result<string>.SuccessAsync(string.Format(_localizer["{0} Saved"], _localizer["Delisting Symbols"]));
                    }
                    else
                    {
                        return await Result<string>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["Delisting Symbols"]));
                    }
                }
                else
                {
                    return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}