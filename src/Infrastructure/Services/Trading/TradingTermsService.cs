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
    public class TradingTermsService : ITradingTermsService
    {
        private readonly IUnitOfWork<string> _strUnitOfWork;
        private readonly IUnitOfWork<int> _intUnitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TradingTermsService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public TradingTermsService(IUnitOfWork<string> strUnitOfWork,
                                   IUnitOfWork<int> intUnitOfWork,
                                   BlazorHeroContext context,
                                   IMapper mapper,
                                   ILogger<TradingTermsService> logger,
                                   IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _strUnitOfWork = strUnitOfWork;
            _intUnitOfWork = intUnitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<TradingTermsDto>> GetTradingTermsAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                                         .AsNoTracking()
                                         .Include(i => i.UPbitKey)
                                         .Include(i => i.Subscription)
                                         .Include(i => i.TradingTerms)
                                         .Include(i => i.ChosenSymbols)
                                         .SingleOrDefaultAsync(p => p.Id.Equals(userId));

                if (user is null)
                {
                    return await Result<TradingTermsDto>.FailAsync(_localizer["User Not Found!"]);
                }
                else
                {

                    var item = _mapper.Map<TradingTermsDto>(user.TradingTerms);

                    item.UPbitKey = _mapper.Map<UPbitKeyDto>(user.UPbitKey);
                    item.ChosenSymbols = user.ChosenSymbols.Select(f => f.market).ToArray();
                    item.MaximumAsset = user.Subscription.MaximumAsset;

                    return await Result<TradingTermsDto>.SuccessAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<TradingTermsDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> UpdateTradingTermsAsync(TradingTermsDto model)
        {
            try
            {
                List<IResult> results = new List<IResult>();

                var user = await _context.Users
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(p => p.Id.Equals(model.UserId));
                if (user is null)
                {
                    return await Result.FailAsync(_localizer["User Not Found!"]);
                }

                results.Add(await UpdateTradingTermsAsync(_mapper.Map<TradingTerms>(model)));
                results.Add(await UpdateChosenSymbolsAsync(model.UserId, model.ChosenSymbols));

                if (results.Count == results.Count(f => f.Succeeded))
                {
                    return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["TradingTerms"]));
                }
                else
                {
                    return await Result.FailAsync(results.Where(f => !f.Succeeded).SelectMany(f => f.Messages).ToList());
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> UpdateTradingTermsAsync(TradingTerms model)
        {
            try
            {
                await _strUnitOfWork.Repository<TradingTerms>().UpdateAsync(model);
                await _strUnitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["TradingTerms"]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        private async Task<IResult> UpdateChosenSymbolsAsync(string userId, IEnumerable<string> models)
        {
            try
            {
                var user = await _context.Users
                                         .Include(i => i.ChosenSymbols)
                                         .SingleOrDefaultAsync(p => p.Id.Equals(userId));

                if (user is null)
                {
                    return await Result<string>.FailAsync(_localizer["User Not Found!"]);
                }

                var registered = user.ChosenSymbols.AsEnumerable();

                var items = (from lt in models
                             from rt in registered.Where(f => f.market.Equals(lt)).DefaultIfEmpty()
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _intUnitOfWork.Repository<ChosenSymbol>().AddAsync(new ChosenSymbol() { market = neo, UserId = userId });
                    }
                }

                var removes = (from lt in registered.AsEnumerable()
                               from rt in models.Where(f => f.Equals(lt.market)).DefaultIfEmpty()
                               where rt == null
                               select lt).ToArray();

                foreach (var remove in removes)
                {
                    await _intUnitOfWork.Repository<ChosenSymbol>().DeleteAsync(remove);
                }

                await _intUnitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Trading.ChosenSymbols"]));
            }
            catch (Exception ex)
            {
                await _strUnitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}