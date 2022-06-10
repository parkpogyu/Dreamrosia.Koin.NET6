using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Shared.Enums;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NETCore.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class MiningBotService : IMiningBotService
    {
        private readonly IUnitOfWork<string> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MiningBotService> _logger;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        private int ElapsedTimes => 100;

        public MiningBotService(IUnitOfWork<string> unitOfWork,
                                BlazorHeroContext context,
                                IMapper mapper,
                                ILogger<MiningBotService> logger,
                                IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<MiningBotDto>>> GetMiningBotsAsync()
        {
            try
            {
                DateTime now = DateTime.Now;

                var items = (from bot in _context.MiningBots
                                                 .AsNoTracking()
                                                 .Include(i => i.User)
                                                 .AsEnumerable()
                             orderby ((BlazorHeroUser)bot.User) is null ? now : ((BlazorHeroUser)bot.User)?.CreatedOn ascending
                             select ((Func<MiningBotDto>)(() =>
                             {
                                 var item = _mapper.Map<MiningBotDto>(bot);

                                 if (bot.User is not null)
                                 {
                                     item.NickName = ((BlazorHeroUser)bot.User).NickName;
                                     item.ProfileImage = ((BlazorHeroUser)bot.User).ProfileImage;
                                 }

                                 return item;
                             }))()).ToArray();

                return await Result<IEnumerable<MiningBotDto>>.SuccessAsync(items);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<MiningBotDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<IEnumerable<MiningBotDto>>> GetTestMiningBotsAsync()
        {
            try
            {
                var items = (from bot in _context.MiningBots
                                           .AsNoTracking()
                                           .Include(i => i.MiningBotTicket)
                                           .AsEnumerable()

                             select ((Func<MiningBotDto>)(() =>
                             {
                                 var item = _mapper.Map<MiningBotDto>(bot);

                                 if (bot.MiningBotTicket is not null)
                                 {
                                     item.Ticket = bot.MiningBotTicket.Id;
                                 }

                                 return item;
                             }))()).OrderByDescending(f => f.Touched ?? new DateTime()).ToArray();

                return await Result<IEnumerable<MiningBotDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<MiningBotDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<TradingTermsExtensionDto>> GetTradingTermsAsync(MiningBotDto model)
        {
            try
            {
                var bot = await GetMiningBotAsync(model);

                if (bot is null)
                {
                    return await Result<TradingTermsExtensionDto>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["MiningBots"]));
                }
                else
                {
                    TradingTermsExtensionDto item;

                    if (string.IsNullOrEmpty(bot.UserId))
                    {
                        item = new TradingTermsExtensionDto()
                        {
                            BotId = bot.Id
                        };
                    }
                    else
                    {
                        item = await GetTradingTermsExtensionAsync(bot);
                    }

                    return await Result<TradingTermsExtensionDto>.SuccessAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<TradingTermsExtensionDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<TradingTermsExtensionDto>> GetTestTradingTermsAsync(MiningBotDto model, string userId)
        {
            try
            {
                var bot = await _context.MiningBots
                                        .SingleOrDefaultAsync(f => f.UserId.Equals(userId));

                if (bot is null)
                {
                    return await Result<TradingTermsExtensionDto>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["MiningBots"]));
                }
                else
                {
                    await TouchMiningBotTicketAsync(model);

                    var item = await GetTradingTermsExtensionAsync(bot);

                    return await Result<TradingTermsExtensionDto>.SuccessAsync(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<TradingTermsExtensionDto>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        private async Task<TradingTermsExtensionDto> GetTradingTermsExtensionAsync(MiningBot bot)
        {
            // Memory leak 발생 ㅠㅠ
            // hubconnection 에서 호출 시 발생하는 것으로 보임 
            //var user = await _context.Users
            //                         .AsNoTracking()
            //                         .Include(i => i.Memberships)
            //                         .Include(i => i.UPbitKey)
            //                         .Include(i => i.TradingTerms)
            //                         .Include(i => i.ChosenSymbols)
            //                         .Include(i => i.Orders)
            //                         .Include(i => i.Transfers)
            //                         .SingleAsync(p => p.Id.Equals(bot.UserId));

            //var deposit = user.Transfers
            //                  .Where(f => f.type == TransferType.deposit)
            //                  .OrderByDescending(f => f.created_at)
            //                  .FirstOrDefault();

            //var withdraw = user.Transfers
            //                   .Where(f => f.type == TransferType.withdraw)
            //                   .OrderByDescending(f => f.created_at)
            //                   .FirstOrDefault();

            // 주문완료(완료/취소) 주문만 조회
            //var order = user.Orders
            //                .Where(f => f.state == OrderState.cancel || f.state == OrderState.done)
            //                .OrderByDescending(f => f.created_at)
            //                .FirstOrDefault();

            //var membership = user.Memberships
            //                     .OrderByDescending(f => f.CreatedOn)
            //                     .First();

            //var item = new TradingTermsExtensionDto()
            //{
            //    BotId = bot.Id
            //};

            var deposit = await _context.Transfers
                                        .AsNoTracking()
                                        .Where(f => f.UserId.Equals(bot.UserId) && f.type == TransferType.deposit)
                                        .OrderByDescending(f => f.created_at)
                                        .FirstOrDefaultAsync();

            var withdraw = await _context.Transfers
                                         .AsNoTracking()
                                         .Where(f => f.UserId.Equals(bot.UserId) && f.type == TransferType.withdraw)
                                         .OrderByDescending(f => f.created_at)
                                         .FirstOrDefaultAsync();

            // 주문완료(완료/취소) 주문만 조회
            var order = await _context.Orders
                                      .AsNoTracking()
                                      .Where(f => f.UserId.Equals(bot.UserId) &&
                                                 (f.state == OrderState.cancel || f.state == OrderState.done))
                                      .OrderByDescending(f => f.created_at)
                                      .FirstOrDefaultAsync();

            var membership = await _context.Memberships
                                           .AsNoTracking()
                                           .Where(f => f.UserId.Equals(bot.UserId))
                                           .OrderByDescending(f => f.CreatedOn)
                                           .FirstOrDefaultAsync();

            var tradingTerms = await _context.TradingTerms
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(f => f.Id.Equals(bot.UserId));

            var upbitkey = await _context.UPbitKeys
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.Id.Equals(bot.UserId));

            var chosenSymbols = await _context.ChosenSymbols
                                              .AsNoTracking()
                                              .Where(f => f.UserId.Equals(bot.UserId))
                                              .ToArrayAsync();

            var item = _mapper.Map<TradingTermsExtensionDto>(tradingTerms);

            item.BotId = bot.Id;

            item.MaximumAsset = membership.MaximumAsset;
            //item.MaximumAsset = 1000000000;

            item.UPbitKey = _mapper.Map<UPbitKeyDto>(upbitkey);
            item.LastOrder = _mapper.Map<OrderDto>(order);
            item.LastDeposit = _mapper.Map<TransferDto>(deposit);
            item.LastWithdraw = _mapper.Map<TransferDto>(withdraw);

            var key = upbitkey.Id.Replace("-", string.Empty);

            item.UPbitKey.access_key = EncryptProvider.AESDecrypt(item.UPbitKey.access_key, key);
            item.UPbitKey.secret_key = EncryptProvider.AESDecrypt(item.UPbitKey.secret_key, key);

            DateTime utc = DateTime.UtcNow.Date;

            var mapped = _mapper.Map<IEnumerable<SeasonSignalDto>>(_context.SeasonSignals
                                                                            .AsNoTracking()
                                                                            .Where(f => f.UserId.Equals(null)));

            var signals = mapped.Where(f => f.UpdatedAt.ToUniversalTime() >= utc)
                                .ToArray();

            if (chosenSymbols.Any())
            {
                item.Signals = (from lt in signals
                                from rt in chosenSymbols.Where(f => f.market.Equals(lt.market)).DefaultIfEmpty()
                                where rt is not null
                                select lt).ToArray();
            }
            else
            {
                item.Signals = signals;
            }

            return item;
        }

        private async Task<MiningBot> GetMiningBotAsync(MiningBotDto model)
        {
            await TouchMiningBotTicketAsync(model);

            var item = await _context.MiningBots
                                     .SingleOrDefaultAsync(p => model.Ticket.Equals(p.Ticket));
            if (item is null)
            {
                await Task.Delay(2000);

                item = await _context.MiningBots
                                     .SingleOrDefaultAsync(p => model.Ticket.Equals(p.Ticket));
            }

            if (item is not null)
            {
                _mapper.Map(model, item);

                await _unitOfWork.Repository<MiningBot>().UpdateAsync(item);
                await _unitOfWork.Commit(new CancellationToken());
            }

            return item;
        }

        private async Task TouchMiningBotTicketAsync(MiningBotDto model)
        {
            var item = await _context.MiningBotTickets
                                     .SingleOrDefaultAsync(p => p.Id.Equals(model.Ticket));

            if (item is null)
            {
                item = new MiningBotTicket()
                {
                    Id = model.Ticket,
                    Touched = DateTime.Now,
                };

                await _unitOfWork.Repository<MiningBotTicket>().AddAsync(item);
            }
            else
            {
                item.Touched = DateTime.Now;

                await _unitOfWork.Repository<MiningBotTicket>().UpdateAsync(item);
            }

            await _unitOfWork.Commit(new CancellationToken());
        }

        public async Task MappingMiningBotAsync()
        {
            DateTime now = DateTime.Now;

            // 휴지 Ticket 삭제
            var removes = _context.MiningBotTickets
                                  .AsEnumerable()
                                  .Where(f => now.Subtract(Convert.ToDateTime(f.Touched)).TotalSeconds > ElapsedTimes)
                                  .ToArray();

            if (removes.Any())
            {
                foreach (var remove in removes)
                {
                    await _unitOfWork.Repository<MiningBotTicket>().DeleteAsync(remove);
                }

                await _unitOfWork.Commit(new CancellationToken());
            }

            var bots = _context.MiningBots
                               .ToArray();

            var unassigned = bots.Where(f => f.Ticket == null)
                                 .ToArray();

            var tickets = (from tkt in _context.MiningBotTickets
                                               .AsNoTracking()
                                               .AsEnumerable()
                           from bot in bots.Where(f => tkt.Id.Equals(f.Ticket)).DefaultIfEmpty()
                           where bot is null
                           select tkt).ToArray().Take(unassigned.Count());

            if (tickets.Any())
            {
                int index = 0;

                foreach (var ticket in tickets)
                {
                    unassigned[index].Ticket = ticket.Id;

                    _logger.LogInformation($"Mapping MiningBot: {unassigned[index].Id} <-> {ticket.Id}");

                    await _unitOfWork.Repository<MiningBot>().UpdateAsync(unassigned[index]);

                    index++;
                }

                await _unitOfWork.Commit(new CancellationToken());
            }
        }
    }
}