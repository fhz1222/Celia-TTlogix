using Application.Common.Models;
using Application.Interfaces.Repositories;
using Application.UseCases.Inventory;
using Application.UseCases.Inventory.Queries.GetInventoryItems;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;
using EFCore = Persistence.EFCoreExtensions;

namespace Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext context;
    private readonly IMapper mapper;
    public InventoryRepository(AppDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public PaginatedList<InventoryItemDto> GetInventoryItems(PaginationQuery pagination,
                                                             InventoryItemDtoFilter filter,
                                                             Func<InventoryItemDto, object>? orderBySelector,
                                                             bool orderByDescending)
    {
        // prepare dictionary for incoming quantity
        Dictionary<(string CustomerCode, string SupplierId, string ProductCode, string WshCode, byte Ownership), int>
            storageDetailsIncomingData = context.TtStorageDetails
            .Where(sd => sd.Status == 0)
            // apply external filter
            .Where(sd => sd.Whscode == filter.WhsCode
                && sd.CustomerCode != null && sd.CustomerCode == filter.CustomerCode
                && (filter.Ownership == null || sd.Ownership == filter.Ownership)
                && (filter.SupplierId == null || EF.Functions.Like(sd.SupplierId, filter.SupplierId.FormatForLikeExpr(), EFCore.ESCAPE_CHAR))
                && (filter.ProductCode == null || EF.Functions.Like(sd.ProductCode, filter.ProductCode.FormatForLikeExpr(), EFCore.ESCAPE_CHAR)))
            .Join(context.TtInbounds.Where(i => i.Status != (byte)Domain.Enums.InboundStatus.Cancelled),
                details => details.InJobNo,
                inbound => inbound.JobNo,
                (details, inbound) => details)
            .GroupBy(sd => new { sd.CustomerCode, sd.SupplierId, sd.ProductCode, sd.Whscode, sd.Ownership })
            .Select(sdg => new
            {
                CustomerCode = sdg.Key.CustomerCode ?? "",
                SupplierId = sdg.Key.SupplierId,
                ProductCode = sdg.Key.ProductCode,
                WhsCode = sdg.Key.Whscode,
                Ownership = sdg.Key.Ownership,
                Qty = (int)sdg.Sum(v => v.OriginalQty)
            })
            .ToDictionary(d => (d.CustomerCode, d.SupplierId, d.ProductCode, d.WhsCode, d.Ownership), d => d.Qty);

        // prepare dictionary for bonded quantity
        var storageDetailsBondedData = context.TtStorageDetails
            .Where(sd => ((sd.Status > (byte)PaletStatus.Incoming
            && sd.Status < (byte)PaletStatus.InTransit)
            || sd.Status == (byte)PaletStatus.Quarantine
            || sd.Status == (byte)PaletStatus.Transferring)
            && sd.BondedStatus == (byte)BondedStatus.Bonded)
            // apply external filter
            .Where(sd => sd.Whscode == filter.WhsCode
                && sd.CustomerCode != null && sd.CustomerCode == filter.CustomerCode
                && (filter.Ownership == null || sd.Ownership == filter.Ownership)
                && (filter.SupplierId == null || EF.Functions.Like(sd.SupplierId, filter.SupplierId.FormatForLikeExpr(), EFCore.ESCAPE_CHAR))
                && (filter.ProductCode == null || EF.Functions.Like(sd.ProductCode, filter.ProductCode.FormatForLikeExpr(), EFCore.ESCAPE_CHAR))
            )
            .Join(context.TtInbounds,
                details => details.InJobNo,
                inbound => inbound.JobNo,
                (details, inbound) => details)
            .GroupBy(sd => new { sd.CustomerCode, sd.SupplierId, sd.ProductCode, sd.Whscode, sd.Ownership })
            .Select(sdg => new
            {
                CustomerCode = sdg.Key.CustomerCode == null ? "" : sdg.Key.CustomerCode,
                SupplierId = sdg.Key.SupplierId,
                ProductCode = sdg.Key.ProductCode,
                WhsCode = sdg.Key.Whscode,
                Ownership = sdg.Key.Ownership,
                Qty = (int)sdg.Sum(v => v.OriginalQty)
            })
            .ToDictionary(d => (d.CustomerCode, d.SupplierId, d.ProductCode, d.WhsCode, d.Ownership), d => d.Qty);

        var mainInventoryData = context.TtInventories
        // apply external filter
        .Where(x => x.Whscode == filter.WhsCode && x.CustomerCode == filter.CustomerCode
            && (filter.Ownership == null || x.Ownership == filter.Ownership)
            && (filter.SupplierId == null || EF.Functions.Like(x.SupplierId, filter.SupplierId.FormatForLikeExpr(), EFCore.ESCAPE_CHAR))
            && (filter.ProductCode == null || EF.Functions.Like(x.ProductCode1, filter.ProductCode.FormatForLikeExpr(), EFCore.ESCAPE_CHAR))
            && (filter.OnHandQty.From == null || x.OnHandQty >= filter.OnHandQty.From) && (filter.OnHandQty.To == null || x.OnHandQty <= filter.OnHandQty.To)
            && (filter.AllocatedQty.From == null || x.AllocatedQty >= filter.AllocatedQty.From) && (filter.AllocatedQty.To == null || x.AllocatedQty <= filter.AllocatedQty.To)
            && (filter.QuarantineQty.From == null || x.QuarantineQty >= filter.QuarantineQty.From) && (filter.QuarantineQty.To == null || x.QuarantineQty <= filter.QuarantineQty.To)
        )
        .Join(context.TtPartMasters,
            inventory => new { inventory.CustomerCode, inventory.SupplierId, inventory.ProductCode1 },
            partmaster => new { partmaster.CustomerCode, partmaster.SupplierId, partmaster.ProductCode1 },
            (inventory, partmaster) => inventory);

        Func<Entities.TtInventory, int> getIncomingQtyFunc = i
            => storageDetailsIncomingData.TryGetValue((i.CustomerCode, i.SupplierId, i.ProductCode1, i.Whscode, i.Ownership), out int qty)
                ? qty : 0;

        Func<Entities.TtInventory, int> getBondedQtyFunc = i
            => storageDetailsBondedData.TryGetValue((i.CustomerCode, i.SupplierId, i.ProductCode1, i.Whscode, i.Ownership), out int qty)
                ? qty : 0;

        Func<Entities.TtInventory, bool> checkIfIncomingQtyConditionIsTrue = i
            => filter.IncomingQty.Check(getIncomingQtyFunc(i));

        Func<Entities.TtInventory, bool> checkIfBondedQtyConditionIsTrue = i 
            => filter.BondedQty.Check(getBondedQtyFunc(i));

        var result = mainInventoryData.AsEnumerable()
            // incoming and bonded filters
            .Where(i => filter.IncomingQty.IsEmpty || checkIfIncomingQtyConditionIsTrue(i))
            .Where(i => filter.BondedQty.IsEmpty || checkIfBondedQtyConditionIsTrue(i))
            .Select(i => mapper.Map<InventoryItemDto>(i).SetBondedAndIncomingQty(getIncomingQtyFunc(i), getBondedQtyFunc(i)))
            // pickableQty and nonBondedQty filters
            .Where(i => filter.PickableQty.Check(i.PickableQty)
                && filter.NonBondedQty.Check(i.NonBondedQty));

        // sort data if required
        if (orderBySelector != null)
            result = !orderByDescending ? result.OrderBy(orderBySelector) : result.OrderByDescending(orderBySelector);

        return result.ToPaginatedList(pagination);
    }

    public InventoryItem? GetInventoryItem(string whsCode, string customerCode, string supplierId, string productCode, Ownership ownership)
    {
        var inventory = context.TtInventories.FirstOrDefault(i => i.Whscode == whsCode
            && i.CustomerCode == customerCode
            && i.SupplierId == supplierId
            && i.ProductCode1 == productCode
            && i.Ownership == (byte) ownership);
        if (inventory == null)
            return null;

        return mapper.Map<InventoryItem>(inventory);
    }

    public async Task Update(InventoryItem inventoryItem)
    {
        var existingObject = await context.TtInventories.FindAsync(inventoryItem.Product.CustomerSupplier.CustomerCode
            , inventoryItem.Product.CustomerSupplier.SupplierId
            , inventoryItem.Product.Code
            , inventoryItem.WhsCode
            , (byte) inventoryItem.Ownership);
        if (existingObject == null)
            throw new EntityDoesNotExistException();

        existingObject.QuarantineQty = inventoryItem.QuarantineQty;
        existingObject.QuarantinePkg = inventoryItem.QuarantinePkg;
        existingObject.OnHandQty = inventoryItem.OnHandQty;
        existingObject.OnHandPkg = inventoryItem.OnHandPkg;
        existingObject.AllocatedQty = inventoryItem.AllocatedQty;
        existingObject.AllocatedPkg = inventoryItem.AllocatedPkg;
    }

}
