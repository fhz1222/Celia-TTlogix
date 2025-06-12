using Application.UseCases.Registration.Queries.GetWarehouseList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetWarehouseListDtoFilterHandler : IFilter<GetWarehouseListDtoFilter, TtWarehouse>
{
    public IQueryable<TtWarehouse> GetFilteredTable(AppDbContext context, GetWarehouseListDtoFilter? filter)
    {
        var table = context.Warehouses.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Code, code => x => x.Code == code)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .OptionalWhere(filter.Type, type => x => x.Type == type)
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
