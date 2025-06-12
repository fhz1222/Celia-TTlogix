namespace Persistence.Entities;

public partial class TtPickingAllocatedPid
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int SerialNo { get; set; }
    public string? Pid { get; set; }
    public decimal? AllocatedQty { get; set; }
    public decimal? PickedQty { get; set; }
}
