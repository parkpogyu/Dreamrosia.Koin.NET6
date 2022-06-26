using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UPbitModels = Dreamrosia.Koin.UPbit.Infrastructure.Models;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class UPbitKeyService : IUPbitKeyService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UPbitKeyService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;


        public UPbitKeyService(IUnitOfWork<string> unitOfWork,
                               BlazorHeroContext context,
                               IMapper mapper,
                               ILogger<UPbitKeyService> logger,
                               IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<UPbitKeyDto>> GetUPbitKeyAsync(string userId)
        {
            try
            {
                var item = await _context.UPbitKeys
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (item is null)
                {
                    return await Result<UPbitKeyDto>.FailAsync(_localizer["User Not Found!"]);
                }

                var mapped = _mapper.Map<UPbitKeyDto>(item);

                var key = userId.Replace("-", string.Empty);

                mapped.access_key = EncryptProvider.AESDecrypt(mapped.access_key, key);
                mapped.secret_key = EncryptProvider.AESDecrypt(mapped.secret_key, key);

                return await Result<UPbitKeyDto>.SuccessAsync(mapped);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UPbitKeyDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<UPbitKeyDto>>> GetUPbitKeysAsync(DateTime head, DateTime rear)
        {
            try
            {
                var items = (from key in _context.UPbitKeys
                                                 .AsNoTracking()
                                                 .Where(f => head.Date <= f.CreatedOn && f.CreatedOn < rear.Date.AddDays(1))
                                                 .Include(i => i.User)
                                                 .AsEnumerable()
                             from code in _context.UserLogins
                                                  .AsNoTracking()
                                                  .Where(f => f.UserId.Equals(key.Id))
                                                  .AsEnumerable()
                             orderby key.expire_at descending
                             select ((Func<UPbitKeyDto>)(() =>
                             {
                                 var item = _mapper.Map<UPbitKeyDto>(key);

                                 item.access_key = string.Empty;
                                 item.secret_key = string.Empty;

                                 item.User.UserCode = code?.ProviderKey;

                                 return item;
                             }))()).ToArray();

                return await Result<IEnumerable<UPbitKeyDto>>.SuccessAsync(_mapper.Map<IEnumerable<UPbitKeyDto>>(items));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<UPbitKeyDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }


        public async Task<IResult> UpdateUPbitKeyAsync(UPbitKeyDto model)
        {
            try
            {
                var item = await _unitOfWork.Repository<UPbitKey>()
                                            .Entities
                                            .SingleOrDefaultAsync(f => f.Id.Equals(model.UserId));

                var key = model.UserId.Replace("-", string.Empty);

                model.access_key = EncryptProvider.AESEncrypt(model.access_key, key);
                model.secret_key = EncryptProvider.AESEncrypt(model.secret_key, key);

                if (item is null)
                {
                    var mapped = _mapper.Map<UPbitKey>(model);

                    await _unitOfWork.Repository<UPbitKey>().AddAsync(mapped);
                }
                else
                {
                    _mapper.Map(model, item);

                    item.IsOccurredFatalError = false;
                    item.FatalError = string.Empty;

                    await _unitOfWork.Repository<UPbitKey>().UpdateAsync(item);
                }

                var ticket = _context.MiningBotTickets.SingleOrDefaultAsync(f => f.UserId.Equals(model.UserId));

                if (ticket is null)
                {
                    _context.Database.ExecuteSqlRaw($"CALL PRC_Assign_MiningBotTicket('{model.UserId}')");
                }

                await _unitOfWork.Commit(new CancellationToken());

                return await Result<string>.SuccessAsync(model.UserId, string.Format(_localizer["{0} Updated"], _localizer["UPbit.Open API Key"]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult> OccurredFatalErrorAsync(string userId, string error)
        {
            try
            {
                var item = await _unitOfWork.Repository<UPbitKey>()
                                            .Entities
                                            .SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (item is not null)
                {
                    item.IsOccurredFatalError = true;
                    item.FatalError = error;

                    await _unitOfWork.Repository<UPbitKey>().UpdateAsync(item);
                    await _unitOfWork.Commit(new CancellationToken());
                }

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<string>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<UPbitKeyTestDto>> TestUPbitKeyAsync(UPbitKeyTestDto model)
        {
            try
            {
                ExchangeClientKeys.SetAuthenticationKey(new UPbitModels.UPbitKey()
                {
                    access_key = model.access_key,
                    secret_key = model.secret_key,
                });

                List<Task> tasks = new List<Task>();

                ExAccessKeys exAccessKeys = new ExAccessKeys();
                ExTransfers exDeposits = new ExTransfers();
                ExTransfers exWithdraws = new ExTransfers();
                ExOrders exOrders = new ExOrders();
                ExOrderPost exOrderPost = new ExOrderPost();
                ExPositions exPositions = new ExPositions();

                var accessKeys = exAccessKeys.GetAccessKeysAsync();
                var deposits = exDeposits.GetTransfersAsync(TransferType.deposit, DateTime.Now.Date);
                var withdraws = exWithdraws.GetTransfersAsync(TransferType.withdraw, DateTime.Now.Date);
                var orders = exOrders.GetWaitingOrdersAsync();
                var orderPost = exOrderPost.OrderPostAsync(new ExOrderPost.ExParameter()
                {
                    market = "KRW-BTC",
                    side = OrderSide.bid,
                    volume = 1,
                    price = 1,
                    ord_type = OrderType.limit,
                });
                var positions = exPositions.GetPositionsAsync();

                tasks.Add(accessKeys);
                tasks.Add(orders);
                tasks.Add(orderPost);
                tasks.Add(deposits);
                tasks.Add(withdraws);
                tasks.Add(positions);

                await Task.WhenAll(tasks).ConfigureAwait(false);

                model.IsAuthenticated = accessKeys.Result.Succeeded;

                if (model.IsAuthenticated)
                {
                    model.expire_at = accessKeys.Result.Data.SingleOrDefault(f => f.access_key.Equals(model.access_key))?.expire_at;
                }
                else
                {
                    model.Messages.Add($"{_localizer["UPbitKey.IsAuthenticated"]}:{accessKeys.Result.FullMessage}");
                }

                model.IsAllowedPositions = positions.Result.Succeeded;

                if (!model.IsAllowedPositions)
                {
                    model.Messages.Add($"{_localizer["UPbitKey.AllowedPositions"]}:{positions.Result.FullMessage}");
                }

                model.IsAllowedOrders = orders.Result.Succeeded;

                if (!model.IsAllowedOrders)
                {
                    model.Messages.Add($"{_localizer["UPbitKey.AllowedOrders"]}:{orders.Result.FullMessage}");
                }

                if (orderPost.Result.Succeeded)
                {
                    model.IsAllowedOrder = orderPost.Result.Succeeded;
                }
                else
                {
                    model.IsAllowedOrder = orderPost.Result.Code.Contains("under_min_total_bid") ? true : false;
                }

                if (!model.IsAllowedOrder)
                {
                    var error = orderPost.Result.Code.Contains("under_min_total_bid") ?
                                string.Empty :
                                orderPost.Result.FullMessage;

                    model.Messages.Add($"{_localizer["UPbitKey.AllowedOrder"]}:{error}");
                }


                model.IsAllowedWithdraws = withdraws.Result.Succeeded;

                if (!model.IsAllowedWithdraws)
                {
                    model.Messages.Add($"{_localizer["UPbitKey.AllowedWithdraws"]}:{withdraws.Result.FullMessage}");
                }

                model.IsAllowedDeposits = deposits.Result.Succeeded;

                if (!model.IsAllowedDeposits)
                {
                    model.Messages.Add($"{_localizer["UPbitKey.AllowedDeposits"]}:{deposits.Result.FullMessage}");
                }

                model.IsPassed = model.IsAllowedPositions &&
                                 model.IsAllowedOrder &&
                                 model.IsAllowedOrders &&
                                 model.IsAllowedWithdraws &&
                                 model.IsAllowedDeposits;

                return await Result<UPbitKeyTestDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<UPbitKeyTestDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }
    }
}