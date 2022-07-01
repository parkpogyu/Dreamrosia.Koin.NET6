using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class BankingTransactionsRequestDto
    {
        public DateTime HeadDate { get; set; }
        public DateTime RearDate { get; set; }
    }
}