using Dreamrosia.Koin.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.DTO
{
    public class TransfersRequestDto
    {
        public string UserId { get; set; }

        public DateTime HeadDate { get; set; }

        public DateTime RearDate { get; set; }

        public IEnumerable<TransferType> TransferTypes { get; set; }

    }
}