using Application.UseCases.StockTake.Queries.GetStockTakeInvalidPid;
using Domain.ValueObjects;
namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeInvalidPidFilterHandler : GetStockTakeItemWithStorageInfoFilterHandler, IFilter<GetStockTakeInvalidPidFilter, StockTakeItemWithStorageInfo>
{
    public IQueryable<StockTakeItemWithStorageInfo> GetFilteredTable(AppDbContext context, GetStockTakeInvalidPidFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var innerFilter = new GetStockTakeItemWithStorageInfoFilter
        {
            JobNo = filter.JobNo,
            MustLocationCodeDiffer = true,
            MustWhsCodeMatch = false,
            InStatus = null,
            NotInStatus = new List<StorageStatus> {
                StorageStatus.Putaway,
                StorageStatus.Picked,
                StorageStatus.Packed,
                StorageStatus.Transferring,
                StorageStatus.Quarantine
            }
        };
        return GetFilteredTable(context, innerFilter);
    }
}
