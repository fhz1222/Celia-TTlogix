using Application.Interfaces.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class PickingRepository : IPickingRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;

    public PickingRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public Outbound? GetOutbound(string outboundJob)
    {
        var outbound = context.TtOutbounds.Find(outboundJob);
        return outbound is null ? null : mapper.Map<Outbound>(outbound);
    }

    public OutboundItem? GetOutboundItem(string outboundJob, int itemNo)
    {
        var item = context.TtOutboundDetails.Find(outboundJob, itemNo);
        return item is null ? null : mapper.Map<OutboundItem>(item);
    }

    public List<OutboundItem> GetOutboundItems(string outboundJob)
    {
        var details = context.TtOutboundDetails
            .Where(od => od.JobNo == outboundJob)
            .ProjectTo<OutboundItem>(mapper.ConfigurationProvider)
            .ToList();
        return details;
    }

    public List<PickingListItem> GetPickingListItems(string outboundJob)
    {
        var pickingListItems = context.TtPickingLists
            .Where(p => p.JobNo == outboundJob)
            .ProjectTo<PickingListItem>(mapper.ConfigurationProvider)
            .ToList();
        return pickingListItems;
    }

    public List<PickingListItem> GetPalletPickingListItems(string palletId)
    {
        var pickingListItems = context.TtPickingLists
            .Where(p => p.Pid == palletId)
            .ProjectTo<PickingListItem>(mapper.ConfigurationProvider)
            .ToList();
        return pickingListItems;
    }

    public List<PickingListItem> GetOpenPalletPickingListItemsByAllocatedPID(string allocatedPID)
    {
        var pickingListItems = context.TtPickingLists
            .Where(p => p.AllocatedPid == allocatedPID)
            .ProjectTo<PickingListItem>(mapper.ConfigurationProvider)
            .AsEnumerable()
            .Where(p => p.IsNotPicked)
            .ToList();
        return pickingListItems;
    }

    public EKanbanItem? GetEKanbanItem(PickingListItem pi)
    {
        var item = context.TtPickingLists
            .Join(context.TtPickingListEkanban,
                p => new { p.JobNo, p.LineItem, p.SeqNo },
                pe => new { pe.JobNo, pe.LineItem, pe.SeqNo },
                (pList, pKanban) => new { pList, pKanban })
            .Join(context.EKanbanDetails,
                p => new { p.pKanban.OrderNo, p.pKanban.ProductCode, p.pKanban.SerialNo },
                e => new { e.OrderNo, e.ProductCode, e.SerialNo },
                (p, items) => new { p.pList, items })
            .Where(p => p.pList.JobNo == pi.JobNo && p.pList.LineItem == pi.LineNo && p.pList.SeqNo == pi.SeqNo)
            .Select(p => p.items)
            .FirstOrDefault();

        return item is null ? null : mapper.Map<EKanbanItem>(item);
    }

    public string GetEKanbanRestriction(string orderNo)
    {
        var instructions = context.EKanbanHeaders.Where(eh => eh.OrderNo == orderNo).FirstOrDefault()?.Instructions;
        return instructions ?? string.Empty;
    }

    public PickingAllocatedPallet? GetPickingAllocatedPallet(PickingListItem item)
    {
        var pickingItem = context.TtPickingAllocatedPids.Find(item.JobNo, item.LineNo, item.SeqNo);
        return pickingItem is null ? null : mapper.Map<PickingAllocatedPallet>(pickingItem);
    }

    public List<PickingAllocatedPallet> GetPickingAllocatedPallets(string pid, int allocQty)
    {
        var pickingAllocPallets = context.TtPickingAllocatedPids
            .Where(p => p.Pid == pid)
            .Where(p => p.AllocatedQty == allocQty)
            .Where(p => p.PickedQty == 0)
            .OrderBy(p => p.JobNo).ThenBy(p => p.LineItem).ThenBy(p => p.SerialNo)
            .ProjectTo<PickingAllocatedPallet>(mapper.ConfigurationProvider)
            .ToList();
        return pickingAllocPallets;
    }

    public List<string> GetAutoallocatedCPartPallets(string productCode, string customerCode, string supplierId, string wh, int qty)
    {
        var pallets = GetAutoallocatedPallets(productCode, customerCode, supplierId, wh)
            .Where(p => p.allocatedQty > qty)
            .Select(p => p.pid)
            .OrderBy(p => p)
            .ToList();
        return pallets;
    }

    public List<string> GetAutoallocatedNonCPartPallets(string productCode, string customerCode, string supplierId, string wh, int qty)
    {
        var pallets = GetAutoallocatedPallets(productCode, customerCode, supplierId, wh)
            .OrderBy(p => p.qty == qty ? 0 : 1)     // match equal qty first
            .ThenBy(p => p.pid)
            .Select(p => p.pid)
            .ToList();
        return pallets;
    }

    public List<PickingListItem> GetPickingItemsOnILogStorage(string jobNo, int iLogStorageCategoryId)
    {
        var locations = context.TtLocations
            .AsNoTracking()
            .Where(lc => lc.ILogLocationCategoryId == iLogStorageCategoryId);

        var pickingListItems = context.TtPickingLists
            .AsNoTracking()
            .Where(p => p.JobNo == jobNo)
            .Join(context.TtStorageDetails.AsNoTracking(),
                pList => pList.AllocatedPid,
                sd => sd.Pid,
                (pList, sd) => new { pList, sd })
            .Where(row => locations.Any(x => x.Code == row.sd.LocationCode && x.Whscode == row.sd.Whscode))
            .Select(row => row.pList)
            .ProjectTo<PickingListItem>(mapper.ConfigurationProvider)
            .ToList();

        return pickingListItems;
    }

    public void Update(PickingListItem p)
    {
        var dbObject = context.TtPickingLists.Find(p.JobNo, p.LineNo, p.SeqNo) ?? throw new EntityDoesNotExistException();
        dbObject.Pid = p.Pid;
        dbObject.Qty = p.Qty;
        dbObject.LocationCode = p.LocationCode;
        dbObject.InboundDate = p.PalletInboundDate;
        dbObject.InboundJobNo = p.PalletInboundJobNo;
        dbObject.PickedDate = p.PickedDate;
        dbObject.PickedBy = p.PickedBy;
        dbObject.DownloadBy = p.PickedBy;
        dbObject.DownloadDate = p.PickedDate;
        dbObject.PackageId = "N/A";
        dbObject.AllocatedPid = p.AllocatedPID;
    }

    public void Update(Outbound o)
    {
        var dbObject = context.TtOutbounds.Find(o.JobNo) ?? throw new EntityDoesNotExistException();
        dbObject.Status = o.Status;
    }

    public void Update(OutboundItem i)
    {
        var dbObject = context.TtOutboundDetails.Find(i.JobNo, i.ItemNo) ?? throw new EntityDoesNotExistException();
        dbObject.Status = i.Status;
        dbObject.PickedQty = i.PickedQty;
        dbObject.Qty = i.Qty;
    }

    public void Update(EKanbanItem e)
    {
        var dbObject = context.EKanbanDetails.Find(e.OrderNo, e.ProductCode, e.SerialNo) ?? throw new EntityDoesNotExistException();
        dbObject.QuantitySupplied = e.SuppliedQty;
    }

    public void Update(PickingAllocatedPallet p)
    {
        var dbObject = context.TtPickingAllocatedPids.Find(p.JobNo, p.LineNo, p.SeqNo) ?? throw new EntityDoesNotExistException();
        dbObject.Pid = p.Pid;
        dbObject.AllocatedQty = p.AllocatedQty;
        dbObject.PickedQty = p.PickedQty;
    }

    private List<(string pid, int qty, int allocatedQty)> GetAutoallocatedPallets(string productCode, string customerCode, string supplierId, string wh)
    {
        var pallets = context.TtStorageDetails
            .Where(p => p.CustomerCode == customerCode && p.SupplierId == supplierId && p.Whscode == wh)
            .Where(p => p.ProductCode == productCode && p.OutJobNo == string.Empty && p.Status == StorageStatus.Allocated)
            .Select(p => new { p.Pid, p.Qty, p.AllocatedQty })
            .AsEnumerable()
            .Select(p => (p.Pid, (int)p.Qty, (int)p.AllocatedQty))
            .ToList();
        return pallets;
    }
}
