using Domain.Enums;

namespace Domain.Entities;

public class StockTransferReversal
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string StfJobNo { get; set; } = null!;
    public string? Reason { get; set; }
    public StockTransferReversalStatus Status { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }

    public void Cancel(string userCode, DateTime now)
    {
        Status = StockTransferReversalStatus.Cancelled;
        CancelledBy = userCode;
        CancelledDate = now;
    }

    public void Complete(string userCode, DateTime now)
    {
        Status = StockTransferReversalStatus.Completed;
        CancelledBy = userCode;
        CancelledDate = now;
    }
}