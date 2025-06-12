using Application.UseCases.StockTake.Queries.GetStockTakeUploadedList;
namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeUploadedListFilterHandler: GetStockTakeItemWithStorageInfoFilterHandler, IFilter<GetStockTakeUploadedListFilter, StockTakeItemWithStorageInfo>
{
    public IQueryable<StockTakeItemWithStorageInfo> GetFilteredTable(AppDbContext context, GetStockTakeUploadedListFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var innerFilter = new GetStockTakeItemWithStorageInfoFilter
        {
            JobNo = filter.JobNo,
            MustLocationCodeDiffer = false,
            MustWhsCodeMatch = false,
            NotInStatus = null,
            InStatus = null
        };
        return GetFilteredTable(context, innerFilter);
    }
}
