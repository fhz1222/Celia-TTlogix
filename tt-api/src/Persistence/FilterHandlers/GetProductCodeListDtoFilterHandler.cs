using Application.UseCases.Registration.Queries.GetProductCodeList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetProductCodeListDtoFilterHandler : IFilter<GetProductCodeListDtoFilter, TtProductCode>
{
    public IQueryable<TtProductCode> GetFilteredTable(AppDbContext context, GetProductCodeListDtoFilter? filter)
    {
        var table = context.ProductCodes.AsNoTracking();

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
