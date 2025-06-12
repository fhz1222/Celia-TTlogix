namespace Domain.Entities;

public class PickingRequestRevisionItem
{
#pragma warning disable CS8618
    public string PickingRequestId { get; set; }
    public string ProductCode { get; set; }
    public int LineNo { get; set; }
    public int Qty { get; set; }
    public string SupplierId { get; set; }
    public string? PalletId { get; set; }
#pragma warning restore CS8618

    public PickingRequestRevisionItem()
    {
        
    }

    public PickingRequestRevisionItem(string pickingRequestId, PickingListItem pickingListItem)
    {
        PickingRequestId = pickingRequestId;
        ProductCode = pickingListItem.ProductCode;
        Qty = pickingListItem.Qty;
        SupplierId = pickingListItem.SupplierId;
        PalletId = string.IsNullOrWhiteSpace(pickingListItem.Pid) ? null : pickingListItem.Pid;
    }
}