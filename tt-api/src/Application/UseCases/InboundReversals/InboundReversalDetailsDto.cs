using Domain.ValueObjects;

namespace Application.UseCases.InboundReversals;

public class InboundReversalDetailsDto
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string InJobNo { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public string? Reason { get; set; }
    public InboundReversalStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}
