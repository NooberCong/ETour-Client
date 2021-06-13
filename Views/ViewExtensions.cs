using System;
using System.Linq;

namespace Client.Views
{
    public static class ViewExtensions
    {
        public static string ToCustomDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString("C");
        }

        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;

            int iNextSpace = input.LastIndexOf(" ", length);

            return string.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

        public static string Repeat(this string input, int times)
        {
            return string.Concat(Enumerable.Repeat(input, times));
        }

        public static string ToRangeString(this Tuple<DateTime, DateTime> range)
        {
            return $"{range.Item1.ToCustomDateString()} - {range.Item2.ToCustomDateString()}";
        }
    }
}
