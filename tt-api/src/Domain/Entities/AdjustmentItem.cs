namespace Domain.Entities;

public class AdjustmentItem
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public string ProductCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string PID { get; set; } = null!;
    public int InitialQty { get; set; }
    public int NewQty { get; set; }
    public int InitialQtyPerPkg { get; set; }
    public int NewQtyPerPkg { get; set; }
    public string? Remarks { get; set; }

    public bool IsPositive => NewQty > InitialQty || NewQtyPerPkg > InitialQtyPerPkg;
    public bool QtyIsValid => NewQty >= 0;
    public bool IsAdjusted => NewQty != InitialQty || NewQtyPerPkg != InitialQtyPerPkg;
}