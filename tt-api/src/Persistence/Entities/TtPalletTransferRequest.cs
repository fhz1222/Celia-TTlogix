namespace Persistence.Entities;
public partial class TtPalletTransferRequest
{
    public string JobNo { get; set; } = null!;
    public string PID { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
}
