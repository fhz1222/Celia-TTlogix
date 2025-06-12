namespace Application.UseCases.InboundReversalItems.Commands.AddNewInboundReversalItemCommand;

public class PIDInfo
{
    public int OriginalQty { get; set; }
    public string Pid { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string InJobNo { get; set; } = null!;
}
