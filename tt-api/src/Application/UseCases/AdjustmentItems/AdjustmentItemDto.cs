namespace Application.UseCases.AdjustmentItems;

public class AdjustmentItemDto
{
    public string JobNo { get; set; } = null!;
    public int? LineItem { get; set; }
    public int NewQty { get; set; }
    public int NewQtyPerPkg { get; set; }
    public string? Remarks { get; set; }
    public string PID { get; set; }
}