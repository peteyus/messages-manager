﻿using System.Diagnostics.CodeAnalysis;

namespace Core.Extensions
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull([NotNull] this object? obj, string? argumentName)
        {
            if (obj == null) throw new ArgumentNullException(argumentName ?? "An unnammed object");
        }

        public static bool IsNull(this object? obj)
        {
            return obj == null;
        }
    }
}
