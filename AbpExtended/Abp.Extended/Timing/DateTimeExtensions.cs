using System;

namespace Abp.Timing
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(this DateTime? dt, string format = "yyyy-MM-dd")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToShortDate(this DateTime dt, string format = "yyyy-MM-dd")
        {
            return dt.ToString(format);
        }

        public static string ToLongDateTime(this DateTime? dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToLongDateTime(this DateTime dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.ToString(format);
        }

        public static string ToShortTime(this DateTime? dt, string format = "HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToShortTime(this DateTime dt, string format = "HH:mm")
        {
            return dt.ToString(format);
        }
    }
}
