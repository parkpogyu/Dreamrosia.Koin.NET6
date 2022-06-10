﻿using System.Collections.Generic;

namespace Dreamrosia.Koin.Application.DTO
{
    public class BackTestReportDto : AssetReportDto
    {
        public IEnumerable<PaperOrderDto> Orders { get; set; } = new List<PaperOrderDto>();

        public IEnumerable<PaperPositionDto> Positions { get; set; } = new List<PaperPositionDto>();
    }
}