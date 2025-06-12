using Application.UseCases.Registration.Queries.GetPackageTypeList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetPackageTypeListDtoFilterHandler : IFilter<GetPackageTypeListDtoFilter, TtPackageType>
{
    public IQueryable<TtPackageType> GetFilteredTable(AppDbContext context, GetPackageTypeListDtoFilter? filter)
    {
        var table = context.PackageTypes.AsNoTracking();

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
