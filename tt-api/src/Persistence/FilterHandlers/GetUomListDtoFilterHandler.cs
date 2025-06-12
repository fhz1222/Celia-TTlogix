using Application.UseCases.Registration.Queries.GetUomList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetUomListDtoFilterHandler : IFilter<GetUomListDtoFilter, TtUOM>
{
    public IQueryable<TtUOM> GetFilteredTable(AppDbContext context, GetUomListDtoFilter? filter)
    {
        var table = context.TtUOM.AsNoTracking();

        if(filter is null)
            return table;

        var query = table
            .OptionalWhere(filter.Code, code => x => EF.Functions.Like(x.Code, code.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR))
            .OptionalWhere(filter.Name, name => x => EF.Functions.Like(x.Name, name.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR))
            .OptionalWhere(filter.Type, type => x => x.Type == type)
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        return query;
    }
}
