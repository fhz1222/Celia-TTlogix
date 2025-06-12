namespace Application.UseCases.StorageDetails;

public class ILogStockSynchronizationPalletDto
{
    public string PID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Quantity { get; set; }
    public string LocationCode { get; set; } = null!;
    public int Quarantine { get; set; }
    public int Ownership { get; set; }
    public string InboundDate { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string SupplierID { get; set; } = null!;
    public bool IsCpart { get;set; }
    public int CPartBoxQty { get; set; }
    public string? ExternalPID { get; set; } = null;
}