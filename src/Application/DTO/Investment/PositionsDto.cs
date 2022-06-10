using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "보유코인 목록")]
    public class PositionsDto
    {
        public string UserId { get; set; }

        public PositionDto KRW { get; set; }

        public IEnumerable<PositionDto> Coins { get; set; }

        public IEnumerable<SymbolDto> Unpositions { get; set; }
    }
}