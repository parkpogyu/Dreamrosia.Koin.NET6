using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Constants.Coin;
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
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly ISymbolService _symbolService;
        private readonly IMapper _mapper;
        private readonly ILogger<PositionService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public PositionService(IUnitOfWork<int> unitOfWork,
                               BlazorHeroContext context,
                               IUPbitTickerService upbitTickerService,
                               ISymbolService symbolService,
                               IMapper mapper,
                               ILogger<PositionService> logger,
                               IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _upbitTickerService = upbitTickerService;
            _symbolService = symbolService;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<PositionsDto>> GetPositionsAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                                         .AsNoTracking()
                                         .Include(i => i.TradingTerms)
                                         .Include(i => i.ChosenSymbols)
                                         .Include(i => i.Positions)
                                         .SingleAsync(f => f.Id.Equals(userId));

                var tickers = (await _upbitTickerService.GetTradePricesAsync()).Data;

                var positions = (from pos in user.Positions
                                 from tik in tickers.Where(f => f.market.Equals($"{pos.unit_currency}-{pos.code}")).DefaultIfEmpty()
                                 select ((Func<PositionDto>)(() =>
                                 {
                                     var item = _mapper.Map<PositionDto>(pos);

                                     item.IsListed = tik is null ? false : true;
                                     item.trade_price = tik is null ? item.trade_price : tik.trade_price;

                                     return item;
                                 }))()).ToArray();

                var response = new PositionsDto()
                {
                    KRW = positions.SingleOrDefault(f => f.code.Equals(Currency.Unit.KRW)),
                    Coins = positions.Where(f => !f.code.Equals(Currency.Unit.KRW))
                };

                var symbols = (await _symbolService.GetSymbolsAsync()).Data;

                IEnumerable<SymbolDto> chosens = null;

                if (user.ChosenSymbols.Any())
                {
                    chosens = (from sym in symbols
                               from chs in user.ChosenSymbols.Where(f => f.market.Equals(sym.market)).DefaultIfEmpty()
                               where chs is not null
                               select sym).ToArray();
                }
                else
                {
                    chosens = symbols;
                }

                if (user.TradingTerms.TimeFrame == TimeFrames.Day)
                {
                    response.Unpositions = (from sym in chosens.Where(f => f.DailySignal == SeasonSignals.GoldenCross)
                                            from pos in user.Positions.Where(f => sym.market.Equals($"{f.unit_currency}-{f.code}")).DefaultIfEmpty()
                                            where pos is null
                                            select sym).ToArray();
                }
                else if (user.TradingTerms.TimeFrame == TimeFrames.Week)
                {
                    response.Unpositions = (from sym in chosens.Where(f => f.WeeklySignal == SeasonSignals.GoldenCross ||
                                                                (f.WeeklySignal == SeasonSignals.Above &&
                                                                 f.DailySignal == SeasonSignals.GoldenCross))
                                            from pos in user.Positions.Where(f => sym.market.Equals($"{f.unit_currency}-{f.code}")).DefaultIfEmpty()
                                            where pos is null
                                            select sym).ToArray();
                }

                return await Result<PositionsDto>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<PositionsDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SavePositionsAsync(string userId, IEnumerable<PositionDto> models)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (user is null)
                {
                    return await Result.FailAsync(_localizer["User Not Found!"]);
                }

                var registered = _unitOfWork.Repository<Position>()
                                            .Entities
                                            .Where(f => f.UserId.Equals(userId));

                var items = (from lt in models
                             from rt in registered.Where(f => f.unit_currency.Equals(lt.unit_currency) &&
                                                              f.code.Equals(lt.code)).DefaultIfEmpty()
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Position>().AddAsync(_mapper.Map<Position>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Position>().UpdateAsync(old);
                    }
                }

                var removes = (from lt in registered.AsEnumerable()
                               from rt in models.Where(f => f.unit_currency.Equals(lt.unit_currency) &&
                                                            f.code.Equals(lt.code)).DefaultIfEmpty()
                               where rt == null
                               select lt).ToArray();

                foreach (var remove in removes)
                {
                    await _unitOfWork.Repository<Position>().DeleteAsync(remove);
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Saved"], _localizer["Positions"]));
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