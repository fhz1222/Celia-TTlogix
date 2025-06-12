using Application.Common.Enums;
using Application.Exceptions;
using Application.UseCases.StockTake.Queries.GetStockTakeStandByLocations;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.FilterHandlers;

class GetStockTakeStandByLocationsFilterHandler : IFilter<GetStockTakeStandByLocationsFilter, TtLocation>
{
    public IQueryable<TtLocation> GetFilteredTable(AppDbContext context, GetStockTakeStandByLocationsFilter? filter)
    {
        var table = context.TtLocations
            .AsNoTracking()
            .Where(x => x.Type == (byte)LocationType.Standby)
            .Where(x => x.Status == (byte)LocationStatus.Active);

        if (filter is null)
            return table;

        var stockTake = context.StockTakes.Find(filter.JobNo)
            ?? throw new ApplicationError($"Cannot find stock take for {filter.JobNo}");

        return table
            .Where(x => x.Whscode == stockTake.Whscode);
    }
}
