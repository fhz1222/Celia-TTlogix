namespace Persistence;

internal static class StringExtensions
{
    /// <summary>
    /// Escapes value for SQL Server LIKE operator with \\
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string EscapeForLikeExpr(this string value)
    {
        return value
            .Replace("%", EFCoreExtensions.ESCAPE_CHAR + "%")
            .Replace("_", EFCoreExtensions.ESCAPE_CHAR + "_")
            .Replace("[", EFCoreExtensions.ESCAPE_CHAR + "[")
            .Replace("]", EFCoreExtensions.ESCAPE_CHAR + "]")
            .Replace("^", EFCoreExtensions.ESCAPE_CHAR + "^");
    }

    /// <summary>
    /// Returns pattern for LIKE expression %VALUE% with escaped special characters
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatForLikeExpr(this string value) => $"%{value.EscapeForLikeExpr()}%";

    /// <summary>
    /// Returns pattern for LIKE expression VALUE% with escaped special characters
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string FormatForLikeExprStartsWith(this string value) => $"{value.EscapeForLikeExpr()}%";
}
