namespace Persistence.Entities;

public class ILogPickingRequestRevisionItem
{
    public string PickingRequestId { get; set; } = null!;
    public int PickingRequestRevision { get; set; }
    public int LineNo { get; set; }
    public string SupplierId { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Qty { get; set; }
    public string? Pid { get; set; }
}