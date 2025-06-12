namespace Domain.Entities;

public class InboundReversalDetail
{
    public string JobNo { get; set; } = null!;
    public string Pid { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public double OriginalQty { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
}