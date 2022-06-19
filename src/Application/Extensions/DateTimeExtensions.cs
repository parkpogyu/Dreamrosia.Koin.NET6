using Dreamrosia.Koin.Domain.Enums;
using System;

namespace Dreamrosia.Koin.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? GetBefore(this DateTime dt, DateRangeTerms terms)
        {
            switch (terms)
            {
                case DateRangeTerms._1W:
                    return dt.Date.AddDays(-6);
                case DateRangeTerms._1M:
                    return dt.Date.AddMonths(-1).AddDays(1);
                case DateRangeTerms._3M:
                    return dt.Date.AddMonths(-3).AddDays(1);
                case DateRangeTerms._6M:
                    return dt.Date.AddMonths(-6).AddDays(1);
                case DateRangeTerms._1Y:
                    return dt.Date.AddYears(-1).AddDays(1);
                case DateRangeTerms._YTD:
                    return new DateTime(dt.Year, 1, 1);
                case DateRangeTerms._All:
                default:
                    return null;
            }
        }
    }
}
