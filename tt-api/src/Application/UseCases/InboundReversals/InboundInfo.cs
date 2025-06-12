using Domain.ValueObjects;

namespace Application.UseCases.InboundReversals;

public class InboundInfo
{
    public string RefNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public InboundStatus Status { get; set; } = null!;
    public InboundType Type { get; set; } = null!;
}
