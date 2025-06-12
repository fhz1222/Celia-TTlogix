using Application.Common.Models;
using System.Linq.Expressions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderByDescOrAsc<T>(this IQueryable<T> query, bool desc, Expression<Func<T, object?>> expression)
    {
        return desc ? query.OrderByDescending(expression) : query.OrderBy(expression);
    }

    public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> query, OrderBy? sorting)
    {
        if(sorting is null)
            return query;

        if(sorting.By is null)
            return query;

        var param = Expression.Parameter(typeof(T));
        return query.OrderByDescOrAsc(
            sorting.Descending,
            Expression.Lambda<Func<T, object?>>(
                Expression.Convert(
                    Expression.PropertyOrField(param, sorting.By),
                    typeof(object)
                ),
                param
            )
        );
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PaginationQuery? pagination, out int count)
    {
        count = query.Count();

        if(pagination is null)
            return query;

        var items = query
            .Skip((pagination.PageNumber - 1) * pagination.ItemsPerPage)
            .Take(pagination.ItemsPerPage);

        return items;
    }
}