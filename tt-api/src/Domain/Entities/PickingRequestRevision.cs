namespace Domain.Entities;

public class PickingRequestRevision
{
#pragma warning disable CS8618
    public string PickingRequestId { get; set; }
    public string OutboundJobNo { get; set; }
    public int Revision { get; set; }
    public DateTime? ClosedOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public bool IsClosed => ClosedOn.HasValue;
    public bool IsOpen => !IsClosed;

    public PickingRequestRevision() { }

#pragma warning restore CS8618

    public PickingRequestRevision(PickingRequestRevision prevRevision, string userCode, DateTime createdDate)
    {
        PickingRequestId = prevRevision.PickingRequestId;
        OutboundJobNo = prevRevision.OutboundJobNo;
        Revision = prevRevision.Revision + 1;
        CreatedBy = userCode;
        CreatedOn = createdDate;
        ClosedOn = null;
    }

    public void Close(DateTime closedOn)
    {
        ClosedOn = closedOn;
    }
}
