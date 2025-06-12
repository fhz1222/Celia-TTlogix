using Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetActiveWarehousesComboFilterHandler : IFilter<GetActiveWarehousesComboFilter, TtWarehouse>
{
    public IQueryable<TtWarehouse> GetFilteredTable(AppDbContext context, GetActiveWarehousesComboFilter? filter)
    {
        var table = context.Warehouses.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
