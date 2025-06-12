using Application.UseCases.Customer;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class UomFilterHandler : IFilter<UomFilter, TtUOM>
{
    public IQueryable<TtUOM> GetFilteredTable(AppDbContext context, UomFilter? filter)
    {
        var table = context.TtUOM.AsNoTracking();

        if (filter is null)
            return table;

        byte? statusValue = filter.Status != null ? (byte)filter.Status : null;
        return table
            .OptionalWhere(statusValue, status => x => x.Status == status);
    }
}
