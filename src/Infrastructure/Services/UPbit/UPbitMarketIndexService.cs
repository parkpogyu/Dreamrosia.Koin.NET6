using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitMarketIndexService : IUPbitMarketIndexService
    {
        private readonly BlazorHeroContext _context;
        private readonly IMarketIndexService _marketIndexService;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitMarketIndexService> _logger;

        public UPbitMarketIndexService(BlazorHeroContext context,
                                       IMarketIndexService marketIndexService,
                                       IMapper mapper,
                                       ILogger<UPbitMarketIndexService> logger)
        {
            _context = context;
            _marketIndexService = marketIndexService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult> GetMarketIndicesAsync()
        {
            var last = _context.MarketIndices.OrderByDescending(f => f.candleDateTimeUtc).FirstOrDefault();

            QtMarketIndex QtMarketIndex = new QtMarketIndex();

            QtMarketIndex.QtParameter parameter = new QtMarketIndex.QtParameter();

            DateTime now = DateTime.UtcNow.Date;

            parameter.count = last is null ? parameter.count : (int)now.Subtract(last.candleDateTimeUtc).TotalDays + 1;

            var result = await QtMarketIndex.GetMarketIndicesAsync(parameter);

            if (!result.Succeeded)
            {
                return await Result.FailAsync(result.Messages);
            }

            var items = _mapper.Map<IEnumerable<MarketIndexDto>>(result.Data);

            var saved = await _marketIndexService.SaveMarketIndicesAsync(items);

            if (saved.Succeeded)
            {
                return await Result.SuccessAsync();
            }
            else
            {
                _logger.LogWarning($"SaveMarketIndicesAsync: {saved.FullMessage}");

                return await Result.FailAsync(saved.Messages);
            }
        }
    }
}
