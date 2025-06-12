namespace Domain.Entities;

public class InvoiceRequest
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public string SupplierId { get; set; }
    public string FactoryId { get; set; }
    public string JobNo { get; set; }
    public string SupplierRefNo { get; set; }
    public int? ApprovedBatchId { get; set; }
#pragma warning restore CS8618

    public bool IsCompleted => ApprovedBatchId is { };

    public void Complete(int batchId)
        => ApprovedBatchId = batchId;
}