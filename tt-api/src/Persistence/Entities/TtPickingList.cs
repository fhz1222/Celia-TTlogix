namespace Persistence.Entities;

public partial class TtPickingList
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int SeqNo { get; set; }
    public string ProductCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public decimal Qty { get; set; }
    public string Pid { get; set; } = null!;
    public string Whscode { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public DateTime InboundDate { get; set; }
    public DateTime? ControlDate { get; set; }
    public byte? ControlCodeType { get; set; }
    public string? ControlCode { get; set; }
    public string? ControlCodeValue { get; set; }
    public string DropPoint { get; set; } = null!;
    public string ProductionLine { get; set; } = null!;
    public int? Version { get; set; }
    public string DownloadBy { get; set; } = null!;
    public DateTime? DownloadDate { get; set; }
    public string? PickedBy { get; set; }
    public DateTime? PickedDate { get; set; }
    public string? PackedBy { get; set; }
    public DateTime? PackedDate { get; set; }
    public string InboundJobNo { get; set; } = null!;
    public string? AllocatedPid { get; set; }
}