namespace Domain.Entities;

public class InvoiceBatch
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public string BatchNumber { get; set; }
    public string SupplierId { get; set; }
    public string FactoryId { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedBy { get; set; }
#pragma warning restore CS8618

    public bool IsApproved => ApprovedDate is { };
    public bool IsPendingApproval => ApprovedDate is null && RejectedDate is null;

    public void Approve(DateTime date, string userCode, List<InvoiceRequest> requests)
    {
        if (!IsPendingApproval)
        {
            throw new Exception($"Batch is not pending approval. Batch id {Id}.");
        }
        ApprovedDate = date;
        ApprovedBy = userCode;

        requests.ForEach(r => r.Complete(Id));
    }

    public void Reject(DateTime date, string userCode)
    {
        if (!IsPendingApproval)
        {
            throw new Exception($"Batch is not pending approval. Batch id {Id}.");
        }
        RejectedDate = date;
        RejectedBy = userCode;
    }
}
