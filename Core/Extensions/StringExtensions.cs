using System.Globalization;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static string FormatCurrentCulture(this string str, params string[] formatArguments)
        {
            return string.Format(CultureInfo.CurrentCulture, str, formatArguments);
        }

        public static bool HasValue(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}
