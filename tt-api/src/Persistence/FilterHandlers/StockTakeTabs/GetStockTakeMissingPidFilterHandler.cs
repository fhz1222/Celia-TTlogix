using Application.UseCases.StockTake.Queries.GetStockTakeMissingPid;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeMissingPidFilterHandler : IFilter<GetStockTakeMissingPidFilter, StockTakeItemWithStorageInfo>
{
    public IQueryable<StockTakeItemWithStorageInfo> GetFilteredTable(AppDbContext context, GetStockTakeMissingPidFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var validLocations = new List<StorageStatus>
        {
            StorageStatus.Putaway,
            StorageStatus.Allocated,
            StorageStatus.Picked,
            StorageStatus.Packed
        }
        .Select(x => (byte)x);

        var table = context.TtStorageDetails
            .AsNoTracking()
            .Where(sd =>
                !context.StockTakeDetails
                    .Where(std => std.JobNo == filter.JobNo)
                    .Select(std => std.Pid)
                    .Contains(sd.Pid))
            .Where(sd =>
                context.StockTakes
                    .Where(st => st.JobNo == filter.JobNo)
                    .Select(std => std.LocationCode)
                    .Contains(sd.LocationCode))
            .Where(sd => validLocations.Contains(sd.Status))
            .Select(sd => new StockTakeItemWithStorageInfo
            {
                CustomerCode = sd.CustomerCode ?? "",
                ProductCode = sd.ProductCode,
                SupplierID = sd.SupplierId,
                Pid = sd.Pid,
                Qty = (int)sd.Qty,
                InJobNo = sd.InJobNo,
                InboundDate = sd.InboundDate,
                WhsCode = sd.Whscode,
                LocationCode = sd.LocationCode,
                Status = sd.Status,
                Remark = "Pending relocate to STANDBY location"
            })
            .OrderBy(x => x.CustomerCode)
            .ThenBy(x => x.ProductCode)
            .ThenBy(x => x.SupplierID)
            .ThenBy(x => x.Pid);

        return table;
    }
}
