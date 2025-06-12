namespace Application.UseCases.InvoiceRequest.Queries.GetJobStatus;

public class JobStatusDto
{
    public string Status { get; set; } = default!;
    public bool CanBeBlocked { get; set; } = default;
    public bool CanBeUnblocked { get; set; } = default;
    public bool CanBeRequestedNow { get; set; } = default;
}

public enum InvRequestStatus { Issued, Pending, Blocked, NotApplicable, Completed }
