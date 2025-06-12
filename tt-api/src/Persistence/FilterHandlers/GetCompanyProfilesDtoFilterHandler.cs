using Application.UseCases.CompanyProfiles.Queries.GetCompanyProfiles;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetCompanyProfilesDtoFilterHandler : IFilter<GetCompanyProfilesDtoFilter, TtCompanyProfile>
{
    public IQueryable<TtCompanyProfile> GetFilteredTable(AppDbContext context, GetCompanyProfilesDtoFilter? filter)
    {
        var table = context.CompanyProfiles.AsNoTracking();

        if (filter is null)
            return table;

        byte? statusValue = filter.Status != null ? (byte)filter.Status : null;
        return table
            .OptionalWhere(filter.Code, code => x => x.Code == code)
            .OptionalWhere(filter.Name, name => x => x.Name == name)
            .OptionalWhere(statusValue, status => x => x.Status == status);
    }
}
