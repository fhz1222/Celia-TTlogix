namespace Domain.Entities;

public class PalletTransferRequest
{
    public string JobNo { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
    public string PID { get; set; } = null!;

    public bool IsInProgress => !IsCompleted;
    public bool IsCompleted => CompletedOn != null;
    public void Complete(DateTime completedDate) => CompletedOn = completedDate;
    public void Cancel() => CompletedOn = new DateTime(1900, 01, 01);

    public PalletTransferRequest(string jobNo, string pID, DateTime createdOn, string createdBy, DateTime? completedOn = null)
    {
        JobNo = jobNo;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        CompletedOn = completedOn;
        PID = pID;
    }
}
