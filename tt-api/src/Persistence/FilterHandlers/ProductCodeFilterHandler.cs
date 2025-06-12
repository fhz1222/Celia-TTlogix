using Application.UseCases.Customer;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class ProductCodeFilterHandler : IFilter<ProductCodeFilter, TtProductCode>
{
    public IQueryable<TtProductCode> GetFilteredTable(AppDbContext context, ProductCodeFilter? filter)
    {
        var table = context.ProductCodes.AsNoTracking();

        if (filter is null)
            return table;

        byte? statusValue = filter.Status != null ? (byte)filter.Status : null;
        return table
            .OptionalWhere(statusValue, status => x => x.Status == status);
    }
}
