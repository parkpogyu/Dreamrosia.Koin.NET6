﻿using AutoMapper;
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

                var tickets = (from ticket in _context.MiningBotTickets
                                                      .AsNoTracking()
                                                      .Include(i => i.User)
                                                      .Include(i => i.MiningBot)
                                                      .AsEnumerable()
                               orderby ((BlazorHeroUser)ticket.User) is null ? now : ((BlazorHeroUser)ticket.User)?.CreatedOn ascending
                               select ((Func<MiningBotDto>)(() =>
                               {
                                   var item = _mapper.Map<MiningBotDto>(ticket.MiningBot) ?? new MiningBotDto();

                                   item.Ticket = item.Ticket ?? ticket.Id;

                                   if (ticket.User is not null)
                                   {
                                       item.NickName = ((BlazorHeroUser)ticket.User).NickName;
                                       item.ProfileImage = ((BlazorHeroUser)ticket.User).ProfileImage;
                                   }

                                   return item;
                               }))()).ToArray();

                var unassigned = (from bot in _context.MiningBots
                                                      .AsNoTracking()
                                                      .AsEnumerable()
                                  from ticket in tickets.Where(f => f.Ticket.Equals(bot.Ticket)).DefaultIfEmpty()
                                  where ticket is null
                                  select ((Func<MiningBotDto>)(() =>
                                  {
                                      var item = _mapper.Map<MiningBotDto>(bot);

                                      return item;
                                  }))()).ToArray();

                return await Result<IEnumerable<MiningBotDto>>.SuccessAsync(tickets.Union(unassigned));
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
                await TouchMiningBotAsync(model);

                var bot = await _context.MiningBots
                                        .Include(i => i.MiningBotTicket)
                                        .SingleOrDefaultAsync(f => f.Id.Equals(model.Id));

                if (bot is null)
                {
                    return await Result<TradingTermsExtensionDto>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["MiningBots"]));
                }
                else
                {
                    TradingTermsExtensionDto item;

                    if (string.IsNullOrEmpty(bot.MiningBotTicket?.UserId))
                    {
                        item = new TradingTermsExtensionDto()
                        { 
                            Ticket = bot.Ticket,
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
                var user = await _context.Users
                                         .AsNoTracking()
                                         .Include(i => i.MiningBotTicket).ThenInclude(i => i.MiningBot)
                                         .SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (user is null || user.MiningBotTicket is null)
                {
                    return await Result<TradingTermsExtensionDto>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["MiningBots"]));
                }

                MiningBot bot = user.MiningBotTicket.MiningBot;

                if (bot is null)
                {
                    bot = _mapper.Map<MiningBot>(model);
                    bot.Ticket = user.MiningBotTicket.Id;

                    await _unitOfWork.Repository<MiningBot>().AddAsync(bot);
                }
                else if (bot.Id.Equals(model.Id))
                {
                    _mapper.Map(model, bot);

                    await _unitOfWork.Repository<MiningBot>().UpdateAsync(bot);
                }
                else if (!bot.Id.Equals(model.Id))
                {
                    return await Result<TradingTermsExtensionDto>.FailAsync(string.Format(_localizer["{0} Not Found"], _localizer["MiningBots"]));
                }

                await _unitOfWork.Commit(new CancellationToken());

                bot = await _context.MiningBots
                                    .AsNoTracking()
                                    .Include(i => i.MiningBotTicket)
                                    .SingleAsync(f => f.Id.Equals(model.Id));

                var item = await GetTradingTermsExtensionAsync(bot);

                return await Result<TradingTermsExtensionDto>.SuccessAsync(item);
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
            //                         .SingleAsync(f => f.Id.Equals(bot.UserId));

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

            var userId = bot.MiningBotTicket?.UserId;

            var deposit = await _context.Transfers
                                        .AsNoTracking()
                                        .Where(f => f.UserId.Equals(userId) && f.type == TransferType.deposit)
                                        .OrderByDescending(f => f.created_at)
                                        .FirstOrDefaultAsync();

            var withdraw = await _context.Transfers
                                         .AsNoTracking()
                                         .Where(f => f.UserId.Equals(userId) && f.type == TransferType.withdraw)
                                         .OrderByDescending(f => f.created_at)
                                         .FirstOrDefaultAsync();

            // 주문완료(완료/취소) 주문만 조회
            var order = await _context.Orders
                                      .AsNoTracking()
                                      .Where(f => f.UserId.Equals(userId) &&
                                                 (f.state == OrderState.cancel || f.state == OrderState.done))
                                      .OrderByDescending(f => f.created_at)
                                      .FirstOrDefaultAsync();

            var membership = await _context.Memberships
                                           .AsNoTracking()
                                           .Where(f => f.UserId.Equals(userId))
                                           .OrderByDescending(f => f.CreatedOn)
                                           .FirstOrDefaultAsync();

            var tradingTerms = await _context.TradingTerms
                                             .AsNoTracking()
                                             .SingleOrDefaultAsync(f => f.Id.Equals(userId));

            var upbitkey = await _context.UPbitKeys
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(f => f.Id.Equals(userId));

            var chosenSymbols = await _context.ChosenSymbols
                                              .AsNoTracking()
                                              .Where(f => f.UserId.Equals(userId))
                                              .ToArrayAsync();

            var item = _mapper.Map<TradingTermsExtensionDto>(tradingTerms);

            item.Ticket = bot.Ticket;

            item.MaximumAsset = membership.MaximumAsset;
            //item.MaximumAsset = 1000000000;

            item.LastOrder = _mapper.Map<OrderDto>(order);
            item.LastDeposit = _mapper.Map<TransferDto>(deposit);
            item.LastWithdraw = _mapper.Map<TransferDto>(withdraw);

            item.UPbitKey = _mapper.Map<UPbitKeyDto>(upbitkey);

            if (item.UPbitKey is not null)
            {
                var key = upbitkey.Id.Replace("-", string.Empty);

                item.UPbitKey.access_key = EncryptProvider.AESDecrypt(item.UPbitKey.access_key, key);
                item.UPbitKey.secret_key = EncryptProvider.AESDecrypt(item.UPbitKey.secret_key, key);
            }

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

        private async Task TouchMiningBotAsync(MiningBotDto model)
        {
            var item = await _context.MiningBots
                                     .Include(i => i.MiningBotTicket)
                                     .SingleOrDefaultAsync(f => f.Id.Equals(model.Id));

            if (item is null)
            {
                item = _mapper.Map<MiningBot>(model);

                await _unitOfWork.Repository<MiningBot>().AddAsync(item);
            }
            else
            {
                _mapper.Map(model, item);

                await _unitOfWork.Repository<MiningBot>().UpdateAsync(item);
            }

            await _unitOfWork.Commit(new CancellationToken());
        }

        public async Task MappingMiningBotAsync()
        {
            DateTime now = DateTime.Now;

            // 휴지 Bot 삭제
            var removes = _context.MiningBots
                                  .AsEnumerable()
                                  .Where(f => now.Subtract(Convert.ToDateTime(f.Touched)).TotalSeconds > ElapsedTimes)
                                  .ToArray();

            if (removes.Any())
            {
                _logger.LogInformation($"Remove MiningBots: {removes.Count():N0}");

                foreach (var remove in removes)
                {
                    await _unitOfWork.Repository<MiningBot>().DeleteAsync(remove);
                }

                await _unitOfWork.Commit(new CancellationToken());
            }

            var unassigned = _context.MiningBotTickets
                                     .AsNoTracking()
                                     .Include(i => i.MiningBot)
                                     .Where(i => i.MiningBot == null)
                                     .ToArray();

            var bots = _context.MiningBots
                               .Where(f => string.IsNullOrEmpty(f.Ticket))
                               .Take(unassigned.Count()).ToArray();

            if (bots.Any())
            {
                int index = 0;

                foreach (var bot in bots)
                {
                    bot.Ticket = unassigned[index].Id;

                    _logger.LogInformation($"Mapping MiningBot: {index:D2} {bot.Id} <-> {unassigned[index].Id}");

                    await _unitOfWork.Repository<MiningBot>().UpdateAsync(bot);

                    index++;
                }

                await _unitOfWork.Commit(new CancellationToken());
            }
        }
    }
}