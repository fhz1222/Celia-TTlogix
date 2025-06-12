namespace Application.Extensions;

public static class GenericExtensions
{
    public static bool In<T>(this T value, params T[] values) => values.Any(v => v is { } && v.Equals(value));

    public static bool NotIn<T>(this T value, params T[] values) => !value.In(values);
}