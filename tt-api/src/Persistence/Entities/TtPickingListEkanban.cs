namespace Persistence.Entities;

public partial class TtPickingListEkanban
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int SeqNo { get; set; }
    public string OrderNo { get; set; } = null!;
    public string SerialNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
}
