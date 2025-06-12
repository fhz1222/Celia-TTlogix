using Domain.ValueObjects;

namespace Domain.Entities;

public class OutboundItem
{
#pragma warning disable CS8618
    public string JobNo { get; set; }
    public int ItemNo { get; set; }
    public int Qty { get; set; }
    public int PickedQty { get; set; }
    public OutboundDetailStatus Status { get; set; }
#pragma warning restore CS8618
}