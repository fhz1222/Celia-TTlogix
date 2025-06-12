using Domain.Entities;

namespace Application.UseCases.AdjustmentItems;

public class AdjustmentItemWithPalletDto
{
    public string JobNo { get; set; } = null!;
    public int LineItem { get; set; }
    public int NewQty { get; set; }
    public int NewQtyPerPkg { get; set; }
    public string? Remarks { get; set; }
    public Pallet Pallet { get; set; }
}