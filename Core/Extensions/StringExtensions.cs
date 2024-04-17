using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static string FormatCurrentCulture(this string str, params string[] formatArguments)
        {
            return string.Format(CultureInfo.CurrentCulture, str, formatArguments);
        }

        public static bool HasValue([NotNullWhen(true)] this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static void ThrowIfNullOrEmpty(this string str, string? argumentName)
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(argumentName ?? "An unnammed object");
        }
    }
}
