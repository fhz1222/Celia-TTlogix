namespace Application.UseCases.InvoiceRequest.Queries.GetBatches;

public class InvoiceBatchDto
{
    public int BatchId { get; set; } = default!;
    public string BatchNumber { get; set; } = default!;
    public string SupplierId { get; set; } = default!;
    public string SupplierName { get; set; } = default!;
    public DateTime UploadedOn { get; set; } = default!;
    public string UploadedBy { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? Comment { get; set; }
    public int? TruckDepartureHour { get; set; }
    public DateTime? LoadingEtd { get; set; }
    public List<InvoiceDto> Invoices { get; set; } = default!;
    public List<JobDto> Jobs { get; set; } = default!;
}

public enum InvoiceBatchStatus { Approved, Rejected, PendingApproval }

public class InvoiceDto
{
    public string InvoiceNumber { get; set; } = default!;
    public decimal Value { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public int? FileId { get; set; }
}
