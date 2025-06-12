using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace Persistence;

internal static class QueryableExtensions
{
    /// <summary>
    /// Applies 'Where' filter if 'value' is not null else returns 'source' unchanged.
    /// Parameter 'predicateFactory' should return the predicate expression for 'Where' method given the not nullable 'T value'.
    /// </summary>
    public static IQueryable<TSource> OptionalWhere<TSource, T>(
        this IQueryable<TSource> source,
        T? value,
        Func<T, Expression<Func<TSource, bool>>> predicateFactory)
    {
        if(value is not null)
        {
            var predicate = predicateFactory(value);
            return source.Where(predicate);
        }
        return source;
    }
}
