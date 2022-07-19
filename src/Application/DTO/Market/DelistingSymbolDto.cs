using System;

namespace Dreamrosia.Koin.Application.DTO
{
    public class DelistingSymbolDto
    {
        public string market { get; set; }
        public string korean_name { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public DateTime? CloseAt { get; set; }
    }
}
