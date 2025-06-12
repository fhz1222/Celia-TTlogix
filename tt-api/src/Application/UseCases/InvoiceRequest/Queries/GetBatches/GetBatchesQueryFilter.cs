namespace Application.UseCases.InvoiceRequest.Queries.GetBatches;

public class GetBatchesQueryFilter
{
    public string FactoryId { get; set; } = default!;
    public string? WhsCode { get; set; }
    public string? SupplierId { get; set; } = default!;
    public string? InvoiceNumber { get; set; }
    public string? AsnNo { get; set; }
    public string? PoNo { get; set; }
    public string? DDSTNumber { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
}