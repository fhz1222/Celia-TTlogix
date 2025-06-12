namespace Persistence.Entities;

public class ILogPickingRequestRevision
{
    public string PickingRequestId { get; set; } = null!;
    public int Revision { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public DateTime? ClosedOn { get; set; }
}
