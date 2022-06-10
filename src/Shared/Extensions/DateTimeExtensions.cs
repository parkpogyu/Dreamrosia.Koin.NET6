using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Dreamrosia.Koin.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetWeeks(this DateTime date, DayOfWeek first = DayOfWeek.Monday)
        {
            int weeks = new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, first);

            if (weeks > 50 && date.Month == 1)
            {
                return Convert.ToInt32(string.Format("{0}{1:D2}", date.AddMonths(-1).Year, weeks));
            }
            else
            {
                return Convert.ToInt32(string.Format("{0}{1:D2}", date.Year, weeks));
            }
        }

        public static IEnumerable<DateTime> GetDays(this DateTime date, int count, bool forward = true)
        {
            if (forward)
            {
                return Enumerable.Range(0, count).Select(f => date.AddDays(f));
            }
            else
            {
                return Enumerable.Range(0, count).Select(f => date.AddDays(0 - f));
            }
        }

        public static string ToDate(this DateTime date, bool hyphen = false)
        {
            return hyphen ? date.ToString("yyyy-MM-dd") : date.ToString("yyyyMMdd");
        }

        public static int GetWeekOfMonth(this DateTime dt)
        {
            DateTime now = dt;

            int weekOfDay = (now.Day - 1) % 7;
            int thisWeek = (int)now.DayOfWeek;

            double val = Math.Ceiling((double)now.Day / 7);

            return Convert.ToInt32(weekOfDay > thisWeek ? val++ : val);
        }

        public static DateTime FirstDayOfWeek(this DateTime dt, DayOfWeek first = DayOfWeek.Sunday)
        {
            var diff = dt.DayOfWeek - first;

            if (diff < 0) { diff += 7; }

            return dt.AddDays(-diff).Date;
        }

        public static bool Expired(string timestamp)
        {
            var time = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(timestamp));

            var diff = time - DateTime.UtcNow;

            return diff.TotalMinutes < 1 ? true : false;
        }

        public static DateTime ToUniversalDate(this DateTime dt)
        {
            return dt.ToUniversalTime().Date;
        }
    }
}
