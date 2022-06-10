using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Requests;
using Dreamrosia.Koin.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Client.Infrastructure.Managers
{
    public interface ISettlementManager : IManager
    {
        Task<IResult<IEnumerable<BankingTransactionDto>>> GetBankingTransactionsAsync(BankingTransactionsRequestDto model);

        Task<IResult<int>> ImportBankingTransactionsAsync(UploadRequest model);

        Task<IResult<IEnumerable<PointDto>>> GetPointsAsync(PointsRequestDto model);
    }
}