using Application.UseCases.StorageDetails;

namespace Application.UseCases.ILogIntegration.Commands.GenerateILogStockSyncFile;

internal class ILogStockSyncItem
{
    public string PID { get; set; } = null!;
    public string BoxID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Quantity { get; set; }
    public string LocationCode { get; set; } = null!;
    public string Batch { get; set; } = null!;
    public int Quarantine { get; set; }
    public int Ownership { get; set; }
    public string InboundDate { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string SupplierID { get; set; } = null!;
    public string? ExternalPID { get; set; } = null;

    public ILogStockSyncItem(ILogStockSynchronizationPalletDto pallet, BoxDto? box)
    {
        PID = pallet.PID;
        ProductCode = pallet.ProductCode;
        LocationCode = pallet.LocationCode;
        Batch = string.Empty;
        Quarantine = pallet.Quarantine;
        Ownership = pallet.Ownership;
        InboundDate = pallet.InboundDate;
        CustomerCode = pallet.CustomerCode;
        SupplierID = pallet.SupplierID;
        ExternalPID = pallet.ExternalPID;

        if (box is { })
        {
            BoxID = box.BoxId;
            Quantity = box.Qty;
        }
        else
        {
            BoxID = string.Empty;
            Quantity = pallet.Quantity;
        }
    }
}
