using Application.UseCases.Registration.Queries.GetLocationList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers.Location;

class GetLocationListDtoFilterHandler : IFilter<GetLocationListDtoFilter, TtLocation>
{
    public IQueryable<TtLocation> GetFilteredTable(AppDbContext context, GetLocationListDtoFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var query = context.TtLocations
            .AsNoTracking()
            .OptionalWhere(filter.ILogLocationCategoryId, id => x => x.ILogLocationCategoryId == id)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .OptionalWhere(filter.WhsCode, whsCode => x => x.Whscode == whsCode)
            .OptionalWhere(filter.Type, type => x => x.Type == type)
            .OptionalWhere(filter.IsPriority, p => x => x.IsPriority == p)
            .OptionalWhere(filter.AreaCode, areaCode => x => x.AreaCode == areaCode)
            .OptionalWhere(filter.Status, status => x => x.Status == status);

        query = filter.Code switch
        {
            null => query,
            string s when s.Contains('%') => query.Where(x => x.Code != null && EF.Functions.Like(x.Code, FormatForWildcardSearch(s), EFCoreExtensions.ESCAPE_CHAR)),
            string s => query.Where(l => l.Code == s)
        };

        return query;
    }

    private string FormatForWildcardSearch(string? value)
        => value?.FormatForLikeExprStartsWith().Replace($"{EFCoreExtensions.ESCAPE_CHAR}%", "%") ?? string.Empty;
}
