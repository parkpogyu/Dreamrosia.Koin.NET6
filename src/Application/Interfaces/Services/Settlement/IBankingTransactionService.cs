using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Application.Interfaces.Services
{
    public interface IBankingTransactionService
    {
        Task<IResult<IEnumerable<BankingTransactionDto>>> GetBankingTransactionsAsync(BankingTransactionsRequestDto model);
        Task<IResult<int>> ImportBankingTransactionsAsync(UploadRequest model);
        Task<IResult<int>> SaveBankingTransactionsAsync(string userId, IEnumerable<BankingTransactionDto> models);

    }
}