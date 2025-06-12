using Application.Common.Models;
using PetaPoco;

namespace Persistence.Extensions;

internal static class PaginationExtensions
{
    internal static PaginatedList<K> ToPaginatedList<T, K>(this IEnumerable<T> items, PaginationQuery paginationQuery, Func<T, K> map)
        => new(
            items.Skip((paginationQuery.PageNumber - 1) * paginationQuery.ItemsPerPage).Take(paginationQuery.ItemsPerPage).Select(map).ToList(),
            items.Count(),
            paginationQuery.PageNumber,
            paginationQuery.ItemsPerPage);

    internal static PaginatedList<T> ToPaginatedList<T>(this IEnumerable<T> items, PaginationQuery paginationQuery)
        => new(
            items.Skip((paginationQuery.PageNumber - 1) * paginationQuery.ItemsPerPage).Take(paginationQuery.ItemsPerPage).ToList(),
            items.Count(),
            paginationQuery.PageNumber,
            paginationQuery.ItemsPerPage);

    internal static PaginatedList<T> ToPaginatedList<T>(this Page<T> page)
        => new(
            page.Items,
            (int) page.TotalItems,
            (int) page.CurrentPage,
            (int) page.ItemsPerPage);

    internal static PaginatedList<K> ToPaginatedList<T, K>(this Page<T> page, Func<T, K> map)
    => new (
        page.Items.Select(map).ToList(),
        (int)page.TotalItems,
        (int)page.CurrentPage,
        (int)page.ItemsPerPage);

}
