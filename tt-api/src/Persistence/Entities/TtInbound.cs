namespace Persistence.Entities;
public partial class TtInbound
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string Irno { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public DateTime Eta { get; set; }
    public byte TransType { get; set; }
    public byte Charged { get; set; }
    public string Remark { get; set; } = null!;
    public byte Status { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? PutawayBy { get; set; }
    public DateTime? PutawayDate { get; set; }
    public string SupplierId { get; set; } = null!;
    public string? Currency { get; set; }
    public string? Im4no { get; set; }
}

