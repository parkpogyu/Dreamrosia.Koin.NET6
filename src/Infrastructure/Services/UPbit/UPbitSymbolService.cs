using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitSymbolService : IUPbitSymbolService
    {
        private readonly IUPbitTickerService _upbitTickerService;
        private readonly ISymbolService _symbolService;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitSymbolService> _logger;

        public UPbitSymbolService(IUPbitTickerService upbitTickerService,
                                  ISymbolService symbolService,
                                  IMapper mapper,
                                  ILogger<UPbitSymbolService> logger)
        {
            _upbitTickerService = upbitTickerService;
            _symbolService = symbolService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult> GetSymbolsAsync()
        {
            QtSymbol QtSymbol = new QtSymbol();

            var result = await QtSymbol.GetSymbolsAsync();

            _logger.LogDebug($"GetSymbolsAsync: {result.Succeeded} {result.Data?.Count():N0}");

            if (result.Succeeded)
            {
                var items = _mapper.Map<IEnumerable<SymbolDto>>(result.Data);

                var saved = await _symbolService.SaveSymbolsAsync(items);

                if (saved.Succeeded)
                {
                    _upbitTickerService.RequestTickers(items.Select(f => f.market));

                    return await Result.SuccessAsync();
                }
                else
                {
                    _logger.LogWarning($"SaveSymbolsAsync: {result.FullMessage}");

                    return await Result.FailAsync(saved.Messages);
                }
            }
            else
            {
                _logger.LogWarning($"GetSymbolsAsync: {result.FullMessage}");

                return await Result.FailAsync(result.Messages);
            }
        }
    }
}
