using Application.Exceptions;
using Application.Extensions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Utils;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services.PalletPickerService;

internal class PalletPicker : IPalletPicker
{
    private readonly IDateTime dateTime;
    private readonly IRepository repository;

    public PalletPicker(IDateTime dateTime, IRepository repository)
    {
        this.dateTime = dateTime;
        this.repository = repository;
    }

    public async Task Pick(string palletId, string outboundJob, string? parentPalletId)
    {
        try
        {
            repository.BeginTransaction();
            await PickPallet(palletId, outboundJob, parentPalletId);
            repository.CommitTransaction();
        }
        catch
        {
            repository.RollbackTransaction();
            throw;
        }
    }

    private async Task PickPallet(string palletId, string outboundJob, string? parentPalletId)
    {
        // CPart pallet (box) is already moved to a new pallet entity
        // parentPalletId is the parent pallet of the picked pallet 
        // pallet matches the manual allocation when itself or parentPallet was allocated

        var pickingRepo = repository.Picking;

        // ## Validations
        var outbound = pickingRepo.GetOutbound(outboundJob) ?? throw new PalletPickerError("Outbound does not exist");
        var isReturnJob = outbound.Type == OutboundType.Return;
        ValidateWithError($"Cannot perform picking because Outbound status is neither New nor PartialPicked", outbound.Status.NotIn(OutboundStatus.New, OutboundStatus.PartialPicked));

        var pallet = repository.StorageDetails.GetPalletDetail(palletId) ?? throw new PalletPickerError("Pallet does not exist");
        ValidateWithError($"Pallet can't be picked because its status is neither Allocated nor Putaway", pallet.Status.NotIn(StorageStatus.Allocated, StorageStatus.Putaway));
        ValidateWithError("No quantity available on pallet", pallet.Qty == 0);
        ValidateWithError("Warehouse mismatch between pallet and outbound", pallet.WhsCode != outbound.Whs);
        ValidateWithError("Customer mismatch between pallet and outbound", pallet.CustomerCode != outbound.CustomerCode);

        var inboundStatus = repository.Inbounds.GetStatus(pallet.InboundJobNo);
        ValidateWithError("Pallet comes from a cancelled inbound", inboundStatus == InboundStatus.Cancelled);

        var outboundPickingList = pickingRepo.GetPickingListItems(outboundJob);
        ValidateWithError("Empty picking list", outboundPickingList.None());

        var location = repository.Locations.GetLocation(pallet.Location, pallet.WhsCode) ?? throw new PalletPickerError("Can't retrieve location");
        var isInboundLocation = repository.Locations.IsILogInboundLocation(location.Code, location.WarehouseCode);
        ValidateWithError($"Pallet on location {pallet.Location} can't be picked", location.Type.In(LocationType.Quarantine, LocationType.Standby));
        ValidateWithError("Can't pick pallet from iLog Inbound Location", isInboundLocation);

        if (outbound.Type.NotIn(OutboundType.Return, OutboundType.Manual))
        {
            var restriction = pickingRepo.GetEKanbanRestriction(outbound.OrderNo);
            ValidateWithError("Allow EHP stock only", restriction == "EHP" && pallet.Ownership != Ownership.EHP);
            ValidateWithError("Allow Supplier stock only", restriction == "Supplier" && pallet.Ownership != Ownership.Supplier);
        }

        // ## Find a matching Picking List Item
        var pickingListsWithPallet = pickingRepo.GetPalletPickingListItems(palletId);
        var isManuallyAllocated = pickingListsWithPallet.Any(p => p.JobNo == outboundJob);

        var alreadyPicked = pickingListsWithPallet.Where(p => p.IsPicked).Any();
        ValidateWithError("Pallet already in Picking List", alreadyPicked);

        var allocatedToOtherOutbound = pickingListsWithPallet.Any(p => p.IsNotPicked && p.JobNo != outboundJob);
        ValidateWithError("Pallet already allocated to other outbound", allocatedToOtherOutbound);

        var relevantOutboundPickingItems = outboundPickingList
            .Where(p => p.ProductCode == pallet.Product.Code)
            .Where(p => p.SupplierId == pallet.SupplierCode)
            .Where(p => p.Whs == pallet.WhsCode)
            .ToList();

        var isCPart = pallet.Product.IsCPart;
        var allocatedQty = pallet.Qty;

        var matchingPickingItems = GetMatchingItems(relevantOutboundPickingItems, pallet, isManuallyAllocated, parentPalletId);
        var selectedPickingItem = matchingPickingItems.FirstOrDefault() ?? throw new PalletPickerError("Unable to find matching picking list item");
        ValidateWithError("Pallet is allocated to another outbound", selectedPickingItem.Pid.IsNotEmpty() && selectedPickingItem.Pid != pallet.Id);
        ValidateWithError("Pallet already picked, auto skip", selectedPickingItem.PickedBy.IsNotEmpty());

        var palletOnReturnLocation = pallet.Location == "RETURN";

        if (pallet.OutboundJobNo.IsNotEmpty()) // ## i.e. pallet was manually allocated to outbound
        {
            ValidateWithError($"Pallet is assigned to outbound {pallet.OutboundJobNo}", pallet.OutboundJobNo != outboundJob);
            ValidateWithError("Retrieved invalid picking list lines", outboundPickingList.Where(p => p.Pid == pallet.Id).Count() != 1);
        }
        else if (pallet.Status == StorageStatus.Putaway)// ## Swapping if scanned label was not allocated to job
        {
            ValidateWithError("Master PID is not allocated", palletOnReturnLocation);

            // ## Get last pallet autoallocated to this job
            var allocatedPalletInDb = pickingRepo.GetPickingAllocatedPallet(selectedPickingItem)?.Pid;
            var allocatedPids = isReturnJob ? pickingRepo.GetAutoallocatedNonCPartPallets(pallet.Product.Code, pallet.CustomerCode, pallet.SupplierCode, pallet.WhsCode, pallet.Qty)
                : allocatedPalletInDb is { } ? new List<string>() { allocatedPalletInDb }
                : isCPart ? pickingRepo.GetAutoallocatedCPartPallets(pallet.Product.Code, pallet.CustomerCode, pallet.SupplierCode, pallet.WhsCode, allocatedQty)
                : pickingRepo.GetAutoallocatedNonCPartPallets(pallet.Product.Code, pallet.CustomerCode, pallet.SupplierCode, pallet.WhsCode, pallet.Qty);
            if (allocatedPids.Any())
            {
                var swappedPallet = repository.StorageDetails.GetPalletDetail(allocatedPids.First())
                    ?? throw new PalletPickerError("Fail to retrieve pallet while performing picking swapping");

                var cPartQtyDiff = 0;

                if (isReturnJob || !isCPart)
                {
                    pallet.Allocate();
                    await repository.StorageDetails.Update(pallet);

                    swappedPallet.Unallocate();
                    await repository.StorageDetails.Update(swappedPallet);
                }
                else
                {
                    pallet.AllocatedQty += allocatedQty;
                    pallet.Status = StorageStatus.Allocated;
                    await repository.StorageDetails.Update(pallet);

                    cPartQtyDiff = Math.Max(allocatedQty - swappedPallet.AllocatedQty, 0);
                    swappedPallet.AllocatedQty = Math.Max(swappedPallet.AllocatedQty - allocatedQty, 0);

                    if (swappedPallet.AllocatedQty == 0)
                    {
                        swappedPallet.Status = StorageStatus.Putaway;
                    }
                    await repository.StorageDetails.Update(swappedPallet);
                }

                // ## Swapped pallet has different qty
                var allocatedQtyDiff = pallet.Qty - swappedPallet.Qty;
                if ((!isCPart || isReturnJob) && allocatedQtyDiff != 0)
                {
                    var kanbanItem = pickingRepo.GetEKanbanItem(selectedPickingItem);
                    if (kanbanItem is { })
                    {
                        kanbanItem.SuppliedQty = pallet.Qty;
                        pickingRepo.Update(kanbanItem);
                    }

                    var outboundItem = pickingRepo.GetOutboundItem(selectedPickingItem.JobNo, selectedPickingItem.LineNo)
                        ?? throw new PalletPickerError($"Failed to obtain outbound item {selectedPickingItem.LineNo}.");
                    outboundItem.Qty = outboundItem.PickedQty = outboundItem.Qty + allocatedQtyDiff;
                    pickingRepo.Update(outboundItem);

                    if (pallet.Ownership == swappedPallet.Ownership)
                    {
                        var inventory = repository.Inventory.GetInventoryItem(pallet.WhsCode, pallet.CustomerCode, pallet.SupplierCode, pallet.Product.Code, pallet.Ownership)
                            ?? throw new PalletPickerError($"No inventory record for product {pallet.Product.Code}");
                        inventory.AllocatedQty += allocatedQtyDiff;
                        await repository.Inventory.Update(inventory);
                    }
                }

                // ## Swapped pallet has different ownership
                if (pallet.Ownership != swappedPallet.Ownership)
                {
                    var palletInventory = repository.Inventory.GetInventoryItem(pallet.WhsCode, pallet.CustomerCode, pallet.SupplierCode, pallet.Product.Code, pallet.Ownership)
                        ?? throw new PalletPickerError($"No inventory record for product {pallet.Product.Code}");

                    var palletAllocQty = palletInventory.AllocatedQty + (isReturnJob ? pallet.Qty : allocatedQty);
                    var palletAllocPkg = palletInventory.AllocatedPkg + (isReturnJob
                        ? Convert.ToInt32(Math.Ceiling((double)pallet.Qty / pallet.QtyPerPkg))
                        : pallet.AllocatedQty == allocatedQty ? 1 : 0);
                    palletInventory.AllocatedQty = palletAllocQty;
                    palletInventory.AllocatedPkg = palletAllocPkg;
                    await repository.Inventory.Update(palletInventory);

                    var swappedPalletInventory = repository.Inventory.GetInventoryItem(swappedPallet.WhsCode, swappedPallet.CustomerCode, swappedPallet.SupplierCode, swappedPallet.Product.Code, swappedPallet.Ownership)
                        ?? throw new PalletPickerError($"No inventory record for product {pallet.Product.Code}");
                    var swappedPalletAllocQty = swappedPalletInventory.AllocatedQty - (isReturnJob ? swappedPallet.Qty : allocatedQty - cPartQtyDiff);
                    var swappedPalletAllocPkg = swappedPalletInventory.AllocatedPkg - (isReturnJob
                        ? Convert.ToInt32(Math.Ceiling((double)swappedPallet.Qty / swappedPallet.QtyPerPkg))
                        : swappedPallet.AllocatedQty == 0 ? 1 : 0);
                    swappedPalletInventory.AllocatedQty = swappedPalletAllocQty;
                    swappedPalletInventory.AllocatedPkg = swappedPalletAllocPkg;
                    await repository.Inventory.Update(swappedPalletInventory);
                }
                else if (pallet.Ownership == swappedPallet.Ownership && cPartQtyDiff > 0 && !palletOnReturnLocation)
                {
                    var inventory = repository.Inventory.GetInventoryItem(pallet.WhsCode, pallet.CustomerCode, pallet.SupplierCode, pallet.Product.Code, pallet.Ownership)
                        ?? throw new PalletPickerError($"No inventory record for product {pallet.Product.Code}");

                    inventory.AllocatedQty += cPartQtyDiff;
                    inventory.AllocatedPkg += Convert.ToInt32(Math.Ceiling((double)pallet.Qty / pallet.QtyPerPkg));
                    await repository.Inventory.Update(inventory);
                }
            }
        }
        else if (pallet.Status == StorageStatus.Allocated && !isCPart && selectedPickingItem.AllocatedPID != pallet.Id) // ## autoallocated in different outbound
        {
            var otherPickingItems = pickingRepo.GetOpenPalletPickingListItemsByAllocatedPID(pallet.Id);
            if (otherPickingItems.Count > 0)
            {
                var otherPickingItem = otherPickingItems[0];

                var otherPallet = repository.StorageDetails.GetPalletDetail(selectedPickingItem.AllocatedPID)
                    ?? throw new PalletPickerError($"Unable to retrieve details for pallet {selectedPickingItem.AllocatedPID}.");

                // update current picking list
                var savedQty = selectedPickingItem.Qty;

                selectedPickingItem.Qty = otherPickingItem.Qty;
                selectedPickingItem.LocationCode = pallet.Location;
                selectedPickingItem.PalletInboundJobNo = pallet.InboundJobNo;
                selectedPickingItem.PalletInboundDate = pallet.InboundDate;
                selectedPickingItem.AllocatedPID = pallet.Id;
                pickingRepo.Update(selectedPickingItem);

                // update other picking list
                otherPickingItem.Qty = savedQty;
                otherPickingItem.LocationCode = otherPallet.Location;
                otherPickingItem.PalletInboundJobNo = otherPallet.InboundJobNo;
                otherPickingItem.PalletInboundDate = otherPallet.InboundDate;
                otherPickingItem.AllocatedPID = otherPallet.Id;
                pickingRepo.Update(otherPickingItem);
            }
        }

        if (isReturnJob)
        {
            pallet.PickToOutbound(outboundJob);
            await repository.StorageDetails.Update(pallet);
        }
        else
        {
            pallet.PickToOutbound(outboundJob);
            await repository.StorageDetails.Update(pallet);

            if (isCPart)
            {
                var pickingAllocatedPallet = pickingRepo.GetPickingAllocatedPallet(selectedPickingItem)
                    ?? throw new PalletPickerError("Unable to retrieve Picking Allocated PID.");

                var swapPickingAllocatedPid = pickingAllocatedPallet.Pid;
                var swapPickingAllocatedQty = pickingAllocatedPallet.AllocatedQty;

                pickingAllocatedPallet.Pid = pallet.Id;
                pickingAllocatedPallet.AllocatedQty = pallet.Qty;
                pickingAllocatedPallet.PickedQty = pallet.Qty;
                pickingRepo.Update(pickingAllocatedPallet);

                // different pallet was allocated
                if (swapPickingAllocatedPid != (parentPalletId ?? palletId))
                {
                    var swapAllocation = pickingRepo.GetPickingAllocatedPallets((parentPalletId ?? palletId), allocatedQty).FirstOrDefault();
                    if (swapAllocation is { })
                    {
                        swapAllocation.Pid = swapPickingAllocatedPid;
                        swapAllocation.AllocatedQty = swapPickingAllocatedQty;
                        pickingRepo.Update(swapAllocation);

                        // update the other picking list & outbound & eKanban
                        var swappedPickingList = pickingRepo.GetPickingListItems(swapAllocation.JobNo)
                            .Where(p => p.LineNo == swapAllocation.LineNo && p.SeqNo == swapAllocation.SeqNo)
                            .FirstOrDefault()
                            ?? throw new PalletPickerError($"Failed to obtain CPart swap picking list item.");
                        swappedPickingList.Qty = swapPickingAllocatedQty;
                        pickingRepo.Update(swappedPickingList);

                        var swappedOutboundItem = pickingRepo.GetOutboundItem(swapAllocation.JobNo, swapAllocation.LineNo)
                            ?? throw new PalletPickerError($"Failed to obtain CPart swap outbound item {swapAllocation.JobNo}.");
                        swappedOutboundItem.Qty = swappedOutboundItem.Qty - allocatedQty + swapPickingAllocatedQty;
                        swappedOutboundItem.PickedQty = swappedOutboundItem.PickedQty - allocatedQty + swapPickingAllocatedQty;
                        pickingRepo.Update(swappedOutboundItem);

                        var swappedEkanbanItem = pickingRepo.GetEKanbanItem(swappedPickingList);
                        if (swappedEkanbanItem is { })
                        {
                            swappedEkanbanItem.SuppliedQty = allocatedQty;
                            pickingRepo.Update(swappedEkanbanItem);
                        }
                    }
                }
                // different qty was allocated
                if (swapPickingAllocatedQty != allocatedQty)
                {
                    var outboundItem = pickingRepo.GetOutboundItem(selectedPickingItem.JobNo, selectedPickingItem.LineNo)
                        ?? throw new PalletPickerError($"Failed to obtain outbound item {selectedPickingItem.LineNo}.");
                    outboundItem.Qty = outboundItem.Qty - swapPickingAllocatedQty + allocatedQty;
                    outboundItem.PickedQty = outboundItem.PickedQty - swapPickingAllocatedQty + allocatedQty;
                    pickingRepo.Update(outboundItem);

                    var kanbanItem = pickingRepo.GetEKanbanItem(selectedPickingItem);
                    if (kanbanItem is { })
                    {
                        kanbanItem.SuppliedQty = allocatedQty;
                        pickingRepo.Update(kanbanItem);
                    }
                }
            }
        }

        if (selectedPickingItem.PickedBy.IsEmpty())
        {
            selectedPickingItem.PickPalletByILog(pallet, dateTime.Now);
            pickingRepo.Update(selectedPickingItem);
        }

        await repository.SaveChangesAsync();

        // ## Update outbound
        var pickedQuantities = pickingRepo.GetPickingListItems(outboundJob).Where(p => p.IsPicked).GroupBy(p => p.LineNo).ToDictionary(g => g.Key, g => g.Sum(p => p.Qty));
        var outboundItems = pickingRepo.GetOutboundItems(outboundJob);

        foreach (var oi in outboundItems)
        {
            if (!pickedQuantities.ContainsKey(oi.ItemNo)) { continue; }

            var pickedQty = pickedQuantities[oi.ItemNo];
            oi.Status = oi.Qty == pickedQty ? OutboundDetailStatus.Picked : OutboundDetailStatus.Picking;
            pickingRepo.Update(oi);
        }

        var isFullyPicked = outboundItems
            .Select(detail => (detail, pickedQty: pickedQuantities.GetValueOrDefault(detail.ItemNo)))
            .All(i => i.detail.Qty == i.pickedQty);
        outbound.Status = isFullyPicked ? OutboundStatus.Picked : OutboundStatus.PartialPicked;
        pickingRepo.Update(outbound);

        await repository.SaveChangesAsync();
    }

    private void ValidateWithError(string message, bool condition)
    {
        if (condition)
        {
            throw new PalletPickerError(message);
        }
    }

    private List<PickingListItem> GetMatchingItems(List<PickingListItem> relevantOutboundPickingItems, Pallet pallet, bool isManuallyAllocated, string? parentPalletId)
    {
        var validItems0 = relevantOutboundPickingItems
           .Where(p => p.IsNotPicked)
           .Where(p => p.AllocatedPID == (parentPalletId ?? pallet.Id))
           .OrderBy(p => p.LineNo).ThenBy(p => p.SeqNo)
           .ToList();
        if(validItems0.Any())
        {
            return validItems0;
        }

        var validItems1 = relevantOutboundPickingItems
           .Where(p => p.IsNotPicked)
           .Where(p => p.PalletInboundDate.Date == pallet.InboundDate.Date)
           .Where(p => p.Qty == pallet.Qty)
           .Where(p => !isManuallyAllocated || p.Pid == (parentPalletId ?? pallet.Id))
           .OrderBy(p => p.LineNo).ThenBy(p => p.SeqNo)
           .ToList();
        if (validItems1.Any())
        {
            return validItems1;
        }

        var validItems2 = relevantOutboundPickingItems
            .Where(p => p.Pid.IsEmpty())
            .Where(p => p.Qty == pallet.Qty)
            .OrderBy(p => p.LineNo).ThenBy(p => p.SeqNo)
            .ToList();
        if (validItems2.Any())
        {
            return validItems2;
        }

        var validItems3 = relevantOutboundPickingItems
            .Where(p => p.Pid.IsEmpty())
            .Where(p => p.IsNotPicked)
            .OrderBy(p => p.LineNo).ThenBy(p => p.SeqNo)
            .ToList();
        if (validItems3.Any())
        {
            return validItems3;
        }

        var validItems4 = relevantOutboundPickingItems
            .Where(p => p.Pid.IsEmpty())
            .OrderBy(p => p.LineNo).ThenBy(p => p.SeqNo)
            .ToList();
        return validItems4;
    }
}