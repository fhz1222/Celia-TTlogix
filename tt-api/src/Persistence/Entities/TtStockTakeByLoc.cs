
namespace Persistence.Entities;

public partial class TtStockTakeByLoc
{
    public string JobNo { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string? Remark { get; set; }
    public byte Status { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisedDate { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string FixExtraBy { get; set; } = null!;
    public DateTime? FixExtraDate { get; set; }
    public string FixMissingBy { get; set; } = null!;
    public DateTime? FixMissingDate { get; set; }
}
