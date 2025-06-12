using Application.UseCases.Registration.Queries.GetAreaList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers.Area;

class GetAreaListDtoFilterHandler : IFilter<GetAreaListDtoFilter, AreaWithTypeName>
{
    public IQueryable<AreaWithTypeName> GetFilteredTable(AppDbContext context, GetAreaListDtoFilter? filter)
    {
        var table = context.Areas
            .AsNoTracking()
            .Join(
                context.AreaTypes,
                area => area.Type,
                areaType => areaType.Code,
                (a, at) => new AreaWithTypeName
                {
                    Code = a.Code,
                    Name = a.Name,
                    Type = at.Name,
                    WhsCode = a.WhsCode,
                    Status = a.Status
                }
            );

        if (filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Code, code => x => x.Code == code)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .OptionalWhere(filter.Type, type => x => x.Type == type)
            .OptionalWhere(filter.WhsCode, whsCode => x => x.WhsCode == whsCode)
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
