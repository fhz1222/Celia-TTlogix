using Domain.ValueObjects;

namespace Application.UseCases.InboundReversals.Commands.CompleteInboundReversalCommand;

public class InboundReversalSummary
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public int TotalDifferent { get; set; }
    public int TotalDiffPkg { get; set; }
    public Ownership Ownership { get; set; } = null!;
}
