namespace Application.UseCases.InvoiceRequest.Queries;

public class JobDto
{
    public int RequestId { get; set; } = default;
    public JobType Type { get; set; } = default!;
    public string JobNo { get; set; } = default!;
    public string DeliveryDocket { get; set; } = default!;
    public List<JobDetailsDto> Details { get; set; } = default!;
}

public enum JobType { Outbound, StockTransfer }

public class JobDetailsDto
{
    public string? AsnNo { get; set; }
    public string ProductCode { get; set; } = default!;
    public decimal Qty { get; set; } = default!;
    public string? PONumber { get; set; }
    public string? POLineNo { get; set; }
}
