using Domain.ValueObjects;

namespace Domain.Metadata;

public class Metadata
{
    public Status Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }

    public static Metadata Create(string userCode, DateTime date)
    {
        return new Metadata
        {
            Status = Status.Active,
            CreatedBy = userCode,
            CreatedDate = date,
            CancelledBy = "",
            CancelledDate = null,
            RevisedBy = "",
            RevisedDate = null,
        };
    }

    public void Revise(string userCode, DateTime date)
    {
        RevisedBy = userCode;
        RevisedDate = date;
    }
}