namespace Application.UseCases.InboundReversalItems;

public class ReversibleInboundItemDto
{
    public string PID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Qty { get; set; }
    public string LocationCode { get; set; } = null!;
}
