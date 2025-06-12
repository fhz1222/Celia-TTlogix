namespace Application.UseCases.InboundReversalItems;

public class InboundReversalItemDto
{
    public string PID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int OriginalQty { get; set; }
}
