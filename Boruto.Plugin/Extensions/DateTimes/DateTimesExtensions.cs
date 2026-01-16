using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Extensions.DateTimes
{
    public static class DateTimesExtensions
    {
        public static DayOfWeek FIRST_DAY_OF_WEEK = DayOfWeek.Monday;
        public static DayOfWeek LAST_DAY_OF_WEEK = DayOfWeek.Sunday;

        public static DateTime StartOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, value.Kind);
        }

        public static DateTime EndOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 0, value.Kind);
        }


        public static DateTime StartOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind);
        }

        public static DateTime EndOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind).AddMonths(1).AddMilliseconds(-1);
        }

        public static DateTime StartOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind);
        }

        public static DateTime EndOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 12, 31, 23, 59, 59, 0, value.Kind);
        }

        public static DateTime StartOfWeek(this DateTime value)
        {
            var result = value;
            while (result.DayOfWeek != FIRST_DAY_OF_WEEK)
            {
                result = result.AddDays(-1);
            }
            return result.StartOfDay();
        }

        public static DateTime EndOfWeek(this DateTime value)
        {
            var result = value;
            while (result.DayOfWeek != LAST_DAY_OF_WEEK)
            {
                result = result.AddDays(1);
            }
            return result.EndOfDay();
        }

        public static DateTime[] Week(this DateTime value)
        {
            var result = new List<DateTime>();
            result.Add(value.FirstDayThisWeek().StartOfDay());
            result.Add(value.LastDayThisWeek().EndOfDay());

            return result.ToArray();
        }

        public static DateTime[] Month(this DateTime value)
        {
            var result = new List<DateTime>();
            var start = new DateTime(value.Year, value.Month, 1, 0, 0, 0, value.Kind);
            result.Add(start);
            result.Add(start.AddMonths(1).AddMilliseconds(-1));
            return result.ToArray();
        }

        public static DateTime[] Year(this DateTime value)
        {
            var result = new List<DateTime>();
            var start = new DateTime(value.Year, 1, 1, 0, 0, 0, value.Kind);
            result.Add(start);
            result.Add(start.AddYears(1).AddMilliseconds(-1));
            return result.ToArray();
        }

        public static DateTime FirstDayThisWeek(this DateTime value)
        {
            var result = value;
            while (result.DayOfWeek != FIRST_DAY_OF_WEEK)
            {
                result = result.AddDays(-1);
            }
            return result;
        }

        public static DateTime LastDayThisWeek(this DateTime value)
        {
            var result = value;
            while (result.DayOfWeek != LAST_DAY_OF_WEEK)
            {
                result = result.AddDays(1);
            }
            return result;
        }
    }
}
