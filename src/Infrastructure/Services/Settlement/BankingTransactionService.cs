using AutoMapper;
using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Interfaces.Repositories;
using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Infrastructure.Contexts;
using Dreamrosia.Koin.Shared.Interfaces.Services;
using Dreamrosia.Koin.Shared.Localization;
using Dreamrosia.Koin.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Services
{
    public class BankingTransactionService : IBankingTransactionService
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly BlazorHeroContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BankingTransactionService> _logger;

        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<SharedLocalizerResources> _localizer;

        public BankingTransactionService(IUnitOfWork<int> unitOfWork,
                                         BlazorHeroContext context,
                                         ILogger<BankingTransactionService> logger,
                                         IMapper mapper,
                                         IExcelService excelService,
                                         IStringLocalizer<SharedLocalizerResources> localizer)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<IResult<IEnumerable<BankingTransactionDto>>> GetBankingTransactionsAsync(BankingTransactionsRequestDto model)
        {
            try
            {
                var items = (from trn in _context.BankingTransactions
                                                 .AsNoTracking()
                                                 .AsEnumerable()
                                                 .Where(f => f.done_at >= model.HeadDate &&
                                                             f.done_at <= model.RearDate.AddDays(1).AddSeconds(-1))
                             from ext in _context.UserLogins
                                                 .AsNoTracking()
                                                 .AsEnumerable()
                                                 .Where(f => f.ProviderKey.Equals(trn.Contents) ||
                                                             f.ProviderKey.Equals(trn.Memo) ||
                                                             f.ProviderKey.Equals(trn.UserCode)).DefaultIfEmpty()
                             from usr in _context.Users
                                                 .AsNoTracking()
                                                 .AsEnumerable()
                                                 .Where(f => f.Id.Equals(ext?.UserId)).DefaultIfEmpty()
                             orderby trn.done_at descending
                             select ((Func<BankingTransactionDto>)(() =>
                             {
                                 var item = _mapper.Map<BankingTransactionDto>(trn);

                                 item.AccountHolder = _mapper.Map<UserProfileDto>(usr);

                                 return item;
                             }))()).ToArray();

                return await Result<IEnumerable<BankingTransactionDto>>.SuccessAsync(items);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<IEnumerable<BankingTransactionDto>>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }

        public async Task<IResult<int>> ImportBankingTransactionsAsync(UploadRequest model)
        {
            try
            {
                var stream = new MemoryStream(model.Data);

                var result = (await _excelService.ImportAsync(stream,
                                                              mappers: new Dictionary<string, Func<DataRow, BankingTransactionDto, object>>
                                                              {
                                                                  {
                                                                      _localizer["BankingTransaction.done_at"],
                                                                      (row,item) =>
                                                                        item.done_at = Convert.ToDateTime(row[_localizer["BankingTransaction.done_at"]].ToString())
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Classification"],
                                                                      (row,item) =>
                                                                        item.Classification = row[_localizer["BankingTransaction.Classification"]].ToString()
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Contents"],
                                                                      (row,item) =>
                                                                        item.Contents = row[_localizer["BankingTransaction.Contents"]].ToString()
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Deposit"],
                                                                      (row,item) =>
                                                                        item.Deposit = double.TryParse(row[_localizer["BankingTransaction.Deposit"]].ToString(), out var value) ? value : 0
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Withdraw"],
                                                                      (row,item) =>
                                                                        item.Withdraw = double.TryParse(row[_localizer["BankingTransaction.Withdraw"]].ToString(), out var value) ? value : 0
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Balance"],
                                                                      (row,item) =>
                                                                        item.Balance = double.TryParse(row[_localizer["BankingTransaction.Balance"]].ToString(), out var value) ? value : 0
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Counterparty"],
                                                                      (row,item) =>
                                                                        item.Counterparty = row[_localizer["BankingTransaction.Counterparty"]].ToString()
                                                                  },
                                                                  {
                                                                      _localizer["BankingTransaction.Memo"],
                                                                      (row,item) =>
                                                                        item.Memo = row[_localizer["BankingTransaction.Memo"]].ToString()
                                                                  },
                                                                  {
                                                                      _localizer["Subscription.UserCode"],
                                                                      (row,item) =>
                                                                        item.UserCode = row.Table.Columns.Contains(_localizer["Subscription.UserCode"]) ?
                                                                                        row[_localizer["Subscription.UserCode"]].ToString() :
                                                                                        string.Empty
                                                                  },
                                                              }, sheetName: "sheet 1", startRow: 6)); ;

                if (result.Succeeded)
                {
                    var items = result.Data;

                    if (!items.Any())
                    {
                        return await Result<int>.SuccessAsync(items.Count());
                    }

                    var min = items.Min(f => f.done_at).AddMinutes(-1);
                    var max = items.Max(f => f.done_at).AddMinutes(1);

                    var registered = _unitOfWork.Repository<BankingTransaction>()
                                                .Entities
                                                .AsNoTracking()
                                                .Where(f => min <= f.done_at && f.done_at <= max);

                    var transactions = (from lt in items
                                        from rt in registered.Where(f => f.done_at == lt.done_at &&
                                                                         f.Balance == lt.Balance).DefaultIfEmpty()
                                        select new { neo = lt, old = rt }).OrderBy(f => f.neo.done_at)
                                                                          .ToArray();

                    foreach (var transaction in transactions)
                    {
                        if (transaction.old is null)
                        {
                            await _unitOfWork.Repository<BankingTransaction>().AddAsync(_mapper.Map<BankingTransaction>(transaction.neo));
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(transaction.neo.UserCode))
                            {
                                transaction.old.UserCode = transaction.neo.UserCode;

                                await _unitOfWork.Repository<BankingTransaction>().UpdateAsync(transaction.old);
                            }
                        }
                    }

                    await _unitOfWork.Commit(new CancellationToken());

                    return await Result<int>.SuccessAsync(items.Count(), string.Format(_localizer["{0} Saved"], _localizer["Transfers"]));
                }
                else
                {
                    return await Result<int>.FailAsync(result.Messages);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<int>.FailAsync(_localizer["An unhandled error has occurred."]);
            }
        }


        public async Task<IResult<int>> SaveBankingTransactionsAsync(string userId, IEnumerable<BankingTransactionDto> models)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(f => f.Id.Equals(userId));

                if (user is null)
                {
                    return await Result<int>.FailAsync(_localizer["User Not Found!"]);
                }

                return await Result<int>.FailAsync(_localizer["User Not Found!"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return await Result<int>.FailAsync(_localizer["An unhandled error has occurred."]);
            }

            //try
            //{
            //    var registered = _unitOfWork.Repository<BankingTransaction>()
            //                                .Entities
            //                                .Where(f => f.UserId.Equals(userId));

            //    var items = (from lt in models
            //                 from rt in registered.Where(f => f.Id.Equals(lt.uuid)).DefaultIfEmpty()
            //                 orderby lt.created_at
            //                 select new { neo = lt, old = rt }).ToArray();

            //    foreach (var item in items)
            //    {
            //        var neo = item.neo;
            //        var old = item.old;

            //        if (old is null)
            //        {
            //            await _unitOfWork.Repository<BankingTransaction>().AddAsync(_mapper.Map<BankingTransaction>(neo));
            //        }
            //        else
            //        {
            //            _mapper.Map(neo, old);

            //            await _unitOfWork.Repository<BankingTransaction>().UpdateAsync(old);
            //        }
            //    }

            //    await _unitOfWork.Commit(new CancellationToken());

            //    return await Result<int>.SuccessAsync(models.Count(), string.Format(_localizer["{0} Saved"], _localizer["BankingTransactions"]));
            //}
            //catch (Exception ex)
            //{
            //    await _unitOfWork.Rollback();

            //    return await Result<int>.FailAsync(ex.Message);
            //}
        }
    }
}