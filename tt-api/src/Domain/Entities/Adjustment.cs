using Domain.ValueObjects;

namespace Domain.Entities;

public class Adjustment
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? ReferenceNo { get; set; }
    public InventoryAdjustmentJobType JobType { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public InventoryAdjustmentStatus Status { get; set; } = null!;
    public string? Reason { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? CancelledBy { get; set; }

    public bool CanComplete => CanEdit;

    public bool CanEdit => !Status.Equals(InventoryAdjustmentStatus.Cancelled)
            && !Status.Equals(InventoryAdjustmentStatus.Completed);

    public bool CanAddItem => CanEdit && !string.IsNullOrEmpty(ReferenceNo);

    public void Complete(string completedBy, DateTime completedDate)
    {
        CompletedBy = completedBy;
        CompletedDate = completedDate;
        Status = InventoryAdjustmentStatus.Completed;
    }

    public void Cancel(string cancelledBy, DateTime cancelledDate)
    {
        CancelledBy = cancelledBy;
        CancelledDate = cancelledDate;
        Status = InventoryAdjustmentStatus.Cancelled;
    }
}
