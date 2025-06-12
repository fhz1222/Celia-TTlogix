using Domain.ValueObjects;

namespace Domain.Metadata;

public class StatusCancel
{
    public Status Status { get; set; } = null!;
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }

    public void ToggleStatusWithCancel(string userCode, DateTime date)
    {
        if(Status == Status.Active)
        {
            Status = Status.Inactive;
            CancelledBy = userCode;
            CancelledDate = date;
        }
        else
        {
            Status = Status.Active;
            CancelledBy = "";
            CancelledDate = null;
        }
    }
}