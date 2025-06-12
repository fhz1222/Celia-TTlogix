using Application.UseCases.StockTake.Queries.GetStockTakeList;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetStockTakeListDtoFilterHandler : IFilter<GetStockTakeListDtoFilter, TtStockTakeByLoc>
{
    public IQueryable<TtStockTakeByLoc> GetFilteredTable(AppDbContext context, GetStockTakeListDtoFilter? filter)
    {
        var table = context.StockTakes.AsNoTracking();

        if (filter is null)
            return table;

        var query = table
            .Where(x => x.Whscode == filter.WhsCode)
            .OptionalWhere(filter.JobNo, jobNo => x => EF.Functions.Like(x.JobNo, jobNo.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR))
            .OptionalWhere(filter.RefNo, refNo => x => EF.Functions.Like(x.RefNo, refNo.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR))
            .OptionalWhere(filter.LocationCode, code => x => EF.Functions.Like(x.LocationCode, code.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR))
            .OptionalWhere(filter.CreatedDate, range => x => x.CreatedDate >= range.From && x.CreatedDate <= range.To)
            .OptionalWhere(filter.Remark, remark => x => EF.Functions.Like(x.Remark != null ? x.Remark : "", remark.FormatForLikeExprStartsWith(), EFCoreExtensions.ESCAPE_CHAR));

        if(filter.Statuses != null && filter.Statuses.Any())
        {
            query = query.Where(x => filter.Statuses.Contains(x.Status));
        }

        return query;
    }
}
