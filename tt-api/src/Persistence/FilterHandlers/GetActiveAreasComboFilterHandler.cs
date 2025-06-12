using Application.UseCases.Registration.Queries.GetActiveAreasCombo;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetActiveAreasComboFilterHandler : IFilter<GetActiveAreasComboFilter, TtArea>
{
    public IQueryable<TtArea> GetFilteredTable(AppDbContext context, GetActiveAreasComboFilter? filter)
    {
        var table = context.Areas.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.WhsCode, whsCode => x => x.WhsCode == whsCode)
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
