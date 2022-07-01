using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class PointsRequestDto
    {
        public string UserId { get; set; }
        public DateTime HeadDate { get; set; }
        public DateTime RearDate { get; set; }
    }
}