using Application.UseCases.Registration.Queries.GetAreaTypeList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetAreaTypeListDtoFilterHandler : IFilter<GetAreaTypeListDtoFilter, TtAreaType>
{
    public IQueryable<TtAreaType> GetFilteredTable(AppDbContext context, GetAreaTypeListDtoFilter? filter)
    {
        var table = context.AreaTypes.AsNoTracking();

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
