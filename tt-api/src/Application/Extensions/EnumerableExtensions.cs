namespace Application.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
        => items.Select((item, index) => (item, index));

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items) action(item);
    }

    public static bool DoesNotContain<TModel>(this IEnumerable<TModel> items, TModel val) => !items.Contains(val);

    public static bool None<T>(this IEnumerable<T> sequence, Func<T, bool> predicate) => !sequence.Any(predicate);

    public static bool None<T>(this IEnumerable<T> sequence) => !sequence.Any();
}
