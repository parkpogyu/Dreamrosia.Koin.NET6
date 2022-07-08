using System;

namespace Dreamrosia.Koin.Bot.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;

            return dt.AddDays(-1 * diff).Date;
        }
    }
}
