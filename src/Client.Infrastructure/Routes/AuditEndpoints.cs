using System;

namespace Dreamrosia.Koin.Client.Infrastructure.Routes
{
    public static class AuditEndpoints
    {
        public static string DownloadFileFiltered(string searchString, bool searchInOldValues = false, bool searchInNewValues = false)
        {
            return $"{DownloadFile}?searchString={searchString}&searchInOldValues={searchInOldValues}&searchInNewValues={searchInNewValues}";
        }

        public static string GetUserAuditTrails(string userId, DateTime head, DateTime rear)
        {
            return $"api/audits?userId={userId}&head={head:d}&rear={rear:d}";
        }
        public static string DownloadFile = "api/audits/export";
    }
}