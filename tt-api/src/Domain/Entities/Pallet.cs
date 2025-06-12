using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Pallet
{
#pragma warning disable CS8618
    /// <summary>
    /// Pallet identifier (PID)
    /// </summary>
    public string Id { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime InboundDate { get; set; }
    public int Qty { get; set; }
    public int QtyPerPkg { get; set; }
    public int OriginalQty { get; set; }
    public int AllocatedQty { get; set; }
    public Product Product { get; set; }
    public StorageStatus Status { get; set; } = null!;
    public Ownership Ownership { get; set; } = null!;
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public string InboundJobNo { get; set; }
    public string OutboundJobNo { get; set; }
    public bool IsVmi { get; set; }
    public DateTime? PutawayDate { get; set; }
    public string PutawayBy { get; set; }
#pragma warning restore CS8618

    public string CustomerCode => Product.CustomerSupplier.CustomerCode;
    public string SupplierCode => Product.CustomerSupplier.SupplierId;

    public bool CanBeAdjusted(string whsCode, string customerCode, InventoryAdjustmentJobType jobType)
    {
        if (WhsCode != whsCode)
            throw new IncorrectPalletException("WHSCode for chosen PID is different than for adjustment");

        if (Product.CustomerSupplier.CustomerCode != customerCode)
            throw new IncorrectPalletException("CustomerCode for chosen PID is different than for adjustment");

        if (jobType.Equals(InventoryAdjustmentJobType.Normal) && !(Status.Equals(StorageStatus.Putaway) || Status.Equals(StorageStatus.Quarantine))
            || jobType.Equals(InventoryAdjustmentJobType.UndoZeroOut) && !(Status.Equals(StorageStatus.ZeroOut)))
            throw new IncorrectStorageDetailStatusException("This PID status is different than expected for this job type of adjustment ");

        return true;
    }

    public bool IsInQuarantine => Status.Equals(StorageStatus.Quarantine);

    public bool CanBeDecanted(string whsCode, string customerCode)
    {
        if (WhsCode != whsCode)
            throw new IncorrectPalletException($"PID '{Id}' was inbounded to warehouse '{WhsCode}' when '{whsCode}' is expected.");
        if (Product.CustomerSupplier.CustomerCode != customerCode)
            throw new IncorrectPalletException($"PID '{Id}' was inbounded for customer '{Product.CustomerSupplier.CustomerCode}' when '{customerCode}' is expected.");
        if (!Status.Equals(StorageStatus.Putaway))
            throw new IncorrectStorageDetailStatusException($"This PID status is different than expected");

        return true;
    }

    public void RelocateTo(Location location)
    {
        Location = location.Code;
        WhsCode = location.WarehouseCode;
    }

    public void Unrestrict()
    {
        Status = StorageStatus.Putaway;
    }
    public void Allocate()
    {
        Status = StorageStatus.Allocated;
        AllocatedQty = Qty;
    }

    public void Unallocate()
    {
        Status = StorageStatus.Putaway;
        AllocatedQty = 0;
    }

    public void PickToOutbound(string outboundJob)
    {
        AllocatedQty = Qty;
        OutboundJobNo = outboundJob;
        Status = StorageStatus.Picked;
    }

    public void TryToRequest()
    {
        if (!CanBeRequested())
            throw new PalletCannotBeRequestedException("Pallet in this state cannot be requested.");
        Restrict();
    }
    private bool CanBeRequested()
    {
        return Status == StorageStatus.Putaway;
    }
    public void Restrict()
    {
        Status = StorageStatus.Restricted;
    }

    public void UndoPutaway()
    {
        Location = string.Empty;
        PutawayBy = string.Empty;
        PutawayDate = null;
    }
}
