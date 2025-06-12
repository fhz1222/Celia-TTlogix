using Domain.ValueObjects;

namespace Domain.Entities;

public class InboundReversal
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string InJobNo { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string? Reason { get; set; }
    public InboundReversalStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }

    public void Cancel(string userCode, DateTime now)
    {
        Status = InboundReversalStatus.Cancelled;
        CancelledBy = userCode;
        CancelledDate = now;
    }

    public void Complete(string userCode, DateTime now)
    {
        Status = InboundReversalStatus.Completed;
        ConfirmedBy = userCode;
        ConfirmedDate = now;
    }
}