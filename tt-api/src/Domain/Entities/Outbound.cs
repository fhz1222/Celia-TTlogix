using Domain.ValueObjects;

namespace Domain.Entities;

public class Outbound
{
#pragma warning disable CS8618
    public string JobNo { get; set; }
    public string CustomerCode { get; set; }
    public string Whs { get; set; }
    public OutboundType Type { get; set; }
    public OutboundStatus Status { get; set; }
    public string OrderNo { get; set; }
    public string Remarks { get; set; }
#pragma warning restore CS8618
}