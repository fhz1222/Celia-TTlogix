using Domain.ValueObjects;

namespace Domain.Metadata;

public class JobMetadata
{
    public JobStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }

    public void Complete(string userCode, DateTime date)
    {
        Status = JobStatus.Completed;
        CompletedBy = userCode;
        CompletedDate = date;
    }

    public void Cancel(string userCode, DateTime date)
    {
        Status = JobStatus.Cancelled;
        CancelledBy = userCode;
        CancelledDate = date;
    }
}