namespace Domain.Entities;

public class PickingListItem
{
#pragma warning disable CS8618
    public string JobNo { get; set; }
    public int LineNo { get; set; }
    public int SeqNo { get; set; }
    public string Whs { get; set; }
    public string SupplierId { get; set; }
    public string ProductCode { get; set; }
    public string Pid { get; set; }
    public int Qty { get; set; }
    public string LocationCode { get; set; }
    public DateTime PalletInboundDate { get; set; }
    public string PalletInboundJobNo { get; set; }
    public DateTime? PickedDate { get; set; }
    public string PickedBy { get; set; }
    public string AllocatedPID { get; set; }

    public bool IsPicked => PickedDate is not null;
    public bool IsNotPicked => !IsPicked;
#pragma warning restore CS8618

    public void PickPalletByILog(Pallet pallet, DateTime pickDate)
    {
        Pid = pallet.Id;
        Qty = pallet.Qty;
        PickedBy = "ILOG";
        PickedDate = pickDate;
        PalletInboundDate = pallet.InboundDate;
        PalletInboundJobNo = pallet.InboundJobNo;
        LocationCode = pallet.Location;
        AllocatedPID = pallet.Id;
    }
}