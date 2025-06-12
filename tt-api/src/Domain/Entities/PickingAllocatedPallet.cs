namespace Domain.Entities;

public class PickingAllocatedPallet
{
#pragma warning disable CS8618
    public string JobNo { get; set; }
    public int LineNo { get; set; }
    public int SeqNo { get; set; }
    public string Pid { get; set; }
    public int AllocatedQty { get; set; }
    public int PickedQty { get; set; }
#pragma warning restore CS8618
}
