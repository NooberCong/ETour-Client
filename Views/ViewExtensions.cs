using Core.Entities;
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

        public static string ToLongCustomDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
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
        public static string Badge(this Booking.BookingStatus status)
        {
            return status switch
            {
                Booking.BookingStatus.Awaiting_Deposit => "badge badge-warning",
                Booking.BookingStatus.Processing => "badge badge-primary",
                Booking.BookingStatus.Awaiting_Payment => "badge badge-warning",
                Booking.BookingStatus.Completed => "badge badge-success",
                Booking.BookingStatus.Canceled => "badge badge-danger",
                _ => ""
            };
        }

        public static string Badge(this Question.QuestionStatus status)
        {
            return status switch
            {
                Question.QuestionStatus.Pending => "badge badge-warning",
                Question.QuestionStatus.Open => "badge badge-primary",
                Question.QuestionStatus.Closed => "badge badge-danger",
                _ => throw new NotImplementedException(),
            };
        }

        public static string ToCustomString(this Enum enumValue) {
            return enumValue.ToString().Replace('_', ' ');
        }
    }
}
