using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.FilterHandlers.StockTakeTabs;

class GetStockTakeItemWithStorageInfoFilterHandler : IFilter<GetStockTakeItemWithStorageInfoFilter, StockTakeItemWithStorageInfo>
{
    public IQueryable<StockTakeItemWithStorageInfo> GetFilteredTable(AppDbContext context, GetStockTakeItemWithStorageInfoFilter? filter)
    {
        if(filter is null)
            throw new ArgumentNullException(nameof(filter));

        var table = context.StockTakeDetails
            .AsNoTracking()
            .Join(
                context.StockTakes,
                std => std.JobNo,
                st => st.JobNo,
                (std, st) => new { std, st })
            .GroupJoin(
                context.TtStorageDetails,
                x => x.std.Pid,
                sd => sd.Pid,
                (x, sds) => new { x.std, x.st, sds })
            .SelectMany(
                x => x.sds.DefaultIfEmpty(),
                (x, sd) => new { x.std, x.st, sd })
            .GroupJoin(
                context.StockTakeRelocationLogs,
                x => new { x.std.JobNo, x.std.Pid },
                strl => new { strl.JobNo, strl.Pid },
                (x, strls) => new { x.std, x.st, x.sd, strls })
            .SelectMany(
                x => x.strls.DefaultIfEmpty(),
                (x, strl) => new { x.std, x.st, x.sd, strl });

        var inStatusValues = filter.InStatus?.Select(x => (byte)x);
        var notInStatusValues = filter.NotInStatus?.Select(x => (byte)x);
        return table
            .Where(x => x.std.JobNo == filter.JobNo)
            .Where(x => !filter.MustWhsCodeMatch || (x.sd != null && (x.sd.Whscode == x.st.Whscode)))
            .Where(x => !filter.MustLocationCodeDiffer || x.sd == null || (x.sd.LocationCode != x.st.LocationCode))
            .OptionalWhere(inStatusValues, statuses => x => x.sd != null && statuses.Contains(x.sd.Status))
            .OptionalWhere(notInStatusValues, statuses => x => x.sd == null || !statuses.Contains(x.sd.Status))
            .Select(x => new StockTakeItemWithStorageInfo
            {
                //storage detail
                CustomerCode = x.sd == null ? "" : (x.sd.CustomerCode ?? ""),
                ProductCode = x.sd == null ? "" : x.sd.ProductCode,
                SupplierID = x.sd == null ? "" : x.sd.SupplierId,
                Pid = x.sd == null ? "" : x.sd.Pid,
                Qty = x.sd == null ? 0 : (int)x.sd.Qty,
                InJobNo = x.sd == null ? "" : x.sd.InJobNo,
                InboundDate = x.sd == null ? null : x.sd.InboundDate,
                WhsCode = x.sd == null ? "" : x.sd.Whscode,
                LocationCode = x.sd == null ? "" : x.sd.LocationCode,
                Status = x.sd == null ? null : x.sd.Status,
                // stock take relocation log
                Remark = (x.strl == null || x.strl.NewLocationCode == null) ? "Pending relocate to STANDBY location" : $"RELOCATED to {x.strl.NewLocationCode}"
            })
            .OrderBy(x => x.CustomerCode)
            .ThenBy(x => x.ProductCode)
            .ThenBy(x => x.SupplierID)
            .ThenBy(x => x.Pid);
    }
}
