using AutoMapper;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.CoinMarketCap.Infrastructure.Clients;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Constants.Coin;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UnlistedSymbolService : IUnlistedSymbolService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UnlistedSymbolService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public UnlistedSymbolService(IUnitOfWork<string> unitOfWork,
                                     BlazorHeroContext context,
                                     IMapper mapper,
                                     ILogger<UnlistedSymbolService> logger,
                                     IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult> FilterUnlistedSymbolsAsync()
        {
            try
            {
                var positions = await _context.Positions
                                              .AsNoTracking()
                                              .Where(f => !f.code.Equals(Currency.Unit.KRW))
                                              .GroupBy(g => g.code)
                                              .Select(f => f.First()).ToArrayAsync();

                var symbols = await _context.Symbols
                                            .AsNoTracking()
                                            .ToArrayAsync();

                var candidates = (from lt in positions
                                  from rt in symbols.Where(f => f.Id.Equals($"{lt.unit_currency}-{lt.code}")).DefaultIfEmpty()
                                  where rt is null
                                  select lt).ToArray();

                var registered = await _context.UnlistedSymbols.AsNoTracking().ToArrayAsync();

                var items = (from lt in candidates
                             from rt in registered.Where(f => f.Id.Equals(lt.code)).DefaultIfEmpty()
                             where rt is null
                             select lt).ToArray();

                foreach (var item in items)
                {
                    await _unitOfWork.Repository<UnlistedSymbol>().AddAsync(new UnlistedSymbol() { Id = item.code });
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

        public async Task<IResult> GetSymbolsIdAsync()
        {
            try
            {
                QtSymbol QtSymbol = new QtSymbol();

                var result = await QtSymbol.GetSymbolsAsync();

                if (result.Succeeded)
                {
                    var registered = await _context.UnlistedSymbols.AsNoTracking().ToArrayAsync();
                    var items = (from lt in registered
                                 from rt in result.Data.Where(f => f.symbol.Equals(lt.Id)).DefaultIfEmpty()
                                 where rt is not null
                                 select ((Func<UnlistedSymbol>)(() =>
                                 {
                                     lt.CoinMarketCapId = rt.id;
                                     lt.english_name = rt.name;
                                     return lt;
                                 }))()).ToArray();

                    foreach (var item in items)
                    {
                        await _unitOfWork.Repository<UnlistedSymbol>().UpdateAsync(item);
                    }

                    await _unitOfWork.Commit(new CancellationToken());

                    return await Result.SuccessAsync(string.Format(_localizer["{0} Updated"], _localizer["Symbols"]));
                }
                else
                {
                    return await Result.FailAsync(result.Messages);
                }
            }
            catch (Exception ex)
            {
                await _unitOfWork.Rollback();

                _logger.LogError(ex, ex.Message);

                return await Result.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> GetPriceAsync()
        {
            try
            {
                var items = await _context.UnlistedSymbols
                                          .AsNoTracking()
                                          .Where(f=>f.CoinMarketCapId != null)
                                          .ToArrayAsync();

                QtCandle QtCandle = new QtCandle();

                DateTime now = DateTime.Now;

                var parameter = new QtCandle.QtParameter();

                parameter.timeStart = ((DateTimeOffset)now.Date.AddDays(-1)).ToUnixTimeSeconds();
                parameter.timeEnd = ((DateTimeOffset)now).ToUnixTimeSeconds();

                foreach (var item in items)
                {
                    var candles = await QtCandle.GetCandlesAsync(parameter);

                    if (!candles.Succeeded) { continue; }

                    var candle = candles.Data.OrderByDescending(f => f.timestamp).FirstOrDefault();

                    if (candle == null) { continue; }    

                    item.open = candle.open;
                    item.low = candle.low;
                    item.high = candle.high;
                    item.close = candle.close;
                    item.timestamp = candle.timestamp;

                    await _unitOfWork.Repository<UnlistedSymbol>().UpdateAsync(item);
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