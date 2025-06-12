using Application.UseCases.Registration.Queries.GetActiveAreaTypesCombo;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetActiveAreaTypesComboFilterHandler : IFilter<GetActiveAreaTypesComboFilter, TtAreaType>
{
    public IQueryable<TtAreaType> GetFilteredTable(AppDbContext context, GetActiveAreaTypesComboFilter? filter)
    {
        var table = context.AreaTypes.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
