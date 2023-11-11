namespace Core.Extensions
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull(this object obj, string? argumentName)
        {
            if (obj == null) throw new ArgumentNullException(argumentName ?? "An unnammed object");
        }
    }
}
