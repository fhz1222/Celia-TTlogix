using Domain.ValueObjects;

namespace Application.UseCases.InboundReversals;

public class InboundReversalDto
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public InboundReversalStatus Status { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? Reason { get; set; }
}
