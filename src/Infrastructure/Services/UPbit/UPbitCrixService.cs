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
    public class UPbitCrixService : IUPbitCrixService
    {
        private readonly ICrixService _crixService;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitCrixService> _logger;

        public UPbitCrixService(ICrixService crixService,
                                IMapper mapper,
                                ILogger<UPbitCrixService> logger)
        {
            _crixService = crixService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IResult<int>> GetCrixesAsync()
        {
            QtCrix QtCrix = new QtCrix();

            var result = await QtCrix.GetCrixesAsync();

            _logger.LogDebug($"GetCrixesAsync: {result.Succeeded} {result.Data?.Count():N0}");

            if (result.Succeeded)
            {
                var items = _mapper.Map<IEnumerable<CrixDto>>(result.Data);

                var saved = await _crixService.SaveCrixesAsync(items);

                if (saved.Succeeded)
                {
                    return await Result<int>.SuccessAsync(items.Count());
                }
                else
                {
                    _logger.LogWarning($"SaveCrixesAsync: {saved.FullMessage}");

                    return await Result<int>.FailAsync(saved.Messages);
                }
            }
            else
            {
                _logger.LogWarning($"GetCrixesAsync: {result.FullMessage}");

                return await Result<int>.FailAsync(result.Messages);
            }
        }
    }
}
