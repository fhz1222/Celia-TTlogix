namespace Persistence.Entities;

public class InvoiceRequest
{
    public int Id { get; set; }
    public string FactoryId { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string JobNo { get; set; } = null!;
    public string SupplierRefNo { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;
    public int? ApprovedBatchId { get; set; }
}
