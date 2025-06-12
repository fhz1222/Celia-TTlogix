using Application.UseCases.Registration.Queries.GetControlCodeList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetControlCodeListDtoFilterHandler : IFilter<GetControlCodeListDtoFilter, TtControlCode>
{
    public IQueryable<TtControlCode> GetFilteredTable(AppDbContext context, GetControlCodeListDtoFilter? filter)
    {
        var table = context.ControlCodes.AsNoTracking();

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
