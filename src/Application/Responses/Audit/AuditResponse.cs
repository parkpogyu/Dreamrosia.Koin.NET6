using Dreamrosia.Koin.Application.DTO;
using Dreamrosia.Koin.Application.Responses.Identity;
using System;

namespace Dreamrosia.Koin.Application.Responses.Audit
{
    public class AuditResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }

        public UserDto User { get; set; }
    }
}