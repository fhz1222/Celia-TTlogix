using System.Diagnostics.CodeAnalysis;

namespace Application.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    public static bool IsNotEmpty([NotNullWhen(true)] this string? value) => !string.IsNullOrWhiteSpace(value);

    public static bool Contains(this string? value, string phrase)
        => value is not null && value.Contains(phrase);

    public static bool ContainsAll(this string value, params string[] values)
        => values.All(v => value.Contains(v));

    public static bool ContainsAny(this string value, params string[] values)
        => values.Any(v => value.Contains(v));
}
