namespace TT.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int length) => value?.Length > length ? value[..length] : value;

        public static string EscapeForLikeExpr(this string value)
        {
            return value
                .Replace("%", EFCoreExtensions.ESCAPE_CHAR + "%")
                .Replace("_", EFCoreExtensions.ESCAPE_CHAR + "_")
                .Replace("[", EFCoreExtensions.ESCAPE_CHAR + "[")
                .Replace("]", EFCoreExtensions.ESCAPE_CHAR + "]")
                .Replace("^", EFCoreExtensions.ESCAPE_CHAR + "^");
        }
    }
}
