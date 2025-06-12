using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface IPickingRepository
{
    Outbound? GetOutbound(string outboundJob);
    OutboundItem? GetOutboundItem(string outboundJob, int itemNo);
    List<OutboundItem> GetOutboundItems(string outboundJob);
    List<PickingListItem> GetPickingListItems(string outboundJob);
    List<PickingListItem> GetPalletPickingListItems(string palletId);
    List<PickingListItem> GetOpenPalletPickingListItemsByAllocatedPID(string allocatedPID);
    EKanbanItem? GetEKanbanItem(PickingListItem pi);
    string GetEKanbanRestriction(string orderNo);
    List<string> GetAutoallocatedCPartPallets(string productCode, string customerCode, string supplierId, string wh, int qty);
    PickingAllocatedPallet? GetPickingAllocatedPallet(PickingListItem item);
    List<PickingAllocatedPallet> GetPickingAllocatedPallets(string pid, int allocQty);
    List<string> GetAutoallocatedNonCPartPallets(string productCode, string customerCode, string supplierId, string wh, int qty);
    List<PickingListItem> GetPickingItemsOnILogStorage(string jobNo, int iLogStorageCategoryId);
    void Update(PickingListItem p);
    void Update(Outbound o);
    void Update(OutboundItem i);
    void Update(EKanbanItem e);
    void Update(PickingAllocatedPallet p);
}
