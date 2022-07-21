using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Common;
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
    public class SymbolService : ISymbolService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly UPbitTickerService _upbitTickerService;
        private readonly ICandleService _candleService;
        private readonly IMapper _mapper;
        private readonly ILogger<SymbolService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public SymbolService(IUnitOfWork<string> unitOfWork,
                             BlazorHeroContext context,
                             IUPbitTickerService upbitTickerService,
                             ICandleService candleService,
                             IMapper mapper,
                             ILogger<SymbolService> logger,
                             IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _upbitTickerService = upbitTickerService as UPbitTickerService;
            _candleService = candleService;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<SymbolDto>>> GetSymbolsAsync()
        {
            try
            {
                var symbols = _context.Symbols;

                var crixes = _mapper.Map<IEnumerable<CrixDto>>(await _context.Crixes
                                                                             .AsNoTracking()
                                                                             .ToArrayAsync());
                var signals = _mapper.Map<IEnumerable<SeasonSignalDto>>(await _context.SeasonSignals
                                                                                      .AsNoTracking()
                                                                                      .Where(f => f.UserId.Equals(null))
                                                                                      .ToArrayAsync());
                var tickers = (await _upbitTickerService.GetTradePricesAsync()).Data;

                var items = (from sym in symbols.AsNoTracking()
                                                .AsEnumerable()
                             from crix in crixes.Where(f => sym.Id.Equals($"{f.unit_currency}-{f.code}")).DefaultIfEmpty()
                             from signal in signals.Where(f => sym.Id.Equals(f.market)).DefaultIfEmpty()
                             from ticker in tickers.Where(f => sym.Id.Equals(f.market)).DefaultIfEmpty()
                             select ((Func<SymbolDto>)(() =>
                             {
                                 var symbol = _mapper.Map<SymbolDto>(sym);

                                 if (crix is CrixDto)
                                 {
                                     _mapper.Map(crix, symbol);
                                 }

                                 if (signal is SeasonSignalDto)
                                 {
                                     _mapper.Map(signal, symbol);
                                 }

                                 if (ticker is TickerDto)
                                 {
                                     symbol.trade_price = ticker.trade_price;
                                     symbol.signed_change_rate = ticker.signed_change_rate;
                                 }

                                 return symbol;

                             }))()).OrderByDescending(f => f.marketCap).ToArray();


                int index = 1;

                foreach (var unchanged in items.Where(f => f.signed_change_rate == 0))
                {
                    var candles = await _candleService.GetCandlesAsync(unchanged.market, DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date);

                    if (candles.Succeeded)
                    {
                        var ynp = candles.Data.Take(2).ToArray();

                        if (ynp.Count() == 2)
                        {
                            unchanged.trade_price = ynp[0].trade_price;
                            unchanged.signed_change_rate = (double)Ratio.ToSignedPercentage(ynp[0].trade_price, ynp[1].trade_price);
                        }
                    }
                }

                return await Result<IEnumerable<SymbolDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<SymbolDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<string>>> GetSymbolCodesAsync()
        {
            try
            {
                var items = _context.Symbols.Select(f => f.Id);

                return await Result<IEnumerable<string>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<string>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> SaveSymbolsAsync(IEnumerable<SymbolDto> models)
        {
            try
            {
                var entities = _unitOfWork.Repository<Symbol>().Entities;

                var items = (from lt in models
                             from rt in entities.Where(f => f.Id.Equals(lt.market)).DefaultIfEmpty()
                             select new { neo = lt, old = rt }).ToArray();

                foreach (var item in items)
                {
                    var neo = item.neo;
                    var old = item.old;

                    if (old is null)
                    {
                        await _unitOfWork.Repository<Symbol>().AddAsync(_mapper.Map<Symbol>(neo));
                    }
                    else
                    {
                        _mapper.Map(neo, old);

                        await _unitOfWork.Repository<Symbol>().UpdateAsync(old);
                    }
                }

                var removes = (from lt in entities.AsEnumerable()
                               from rt in models.Where(f => f.market.Equals(lt.Id)).DefaultIfEmpty()
                               where rt == null
                               select lt).ToArray();

                foreach (var remove in removes)
                {
                    await _unitOfWork.Repository<Symbol>().DeleteAsync(remove);
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Symbols"]));
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