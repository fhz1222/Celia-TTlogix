using Application.UseCases.StockTake.Queries.GetStockTakeAnotherLocPid;
using Domain.ValueObjects;
namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeAnotherLocPidFilterHandler : GetStockTakeItemWithStorageInfoFilterHandler, IFilter<GetStockTakeAnotherLocPidFilter, StockTakeItemWithStorageInfo>
{
    public IQueryable<StockTakeItemWithStorageInfo> GetFilteredTable(AppDbContext context, GetStockTakeAnotherLocPidFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var innerFilter = new GetStockTakeItemWithStorageInfoFilter
        {
            JobNo = filter.JobNo,
            MustLocationCodeDiffer = true,
            MustWhsCodeMatch = true,
            InStatus = new List<StorageStatus> {
                StorageStatus.Putaway,
                StorageStatus.Picked,
                StorageStatus.Packed,
                StorageStatus.Transferring,
                StorageStatus.Quarantine
            },
            NotInStatus = null
        };
        return GetFilteredTable(context, innerFilter);
    }
}
