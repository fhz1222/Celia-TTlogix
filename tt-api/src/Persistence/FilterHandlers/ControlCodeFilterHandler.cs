using Application.UseCases.Customer;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class ControlCodeFilterHandler : IFilter<ControlCodeFilter, TtControlCode>
{
    public IQueryable<TtControlCode> GetFilteredTable(AppDbContext context, ControlCodeFilter? filter)
    {
        var table = context.ControlCodes.AsNoTracking();

        if (filter is null)
            return table;

        byte? statusValue = filter.Status != null ? (byte)filter.Status : null;
        return table
            .OptionalWhere(statusValue, status => x => x.Status == status);
    }
}
