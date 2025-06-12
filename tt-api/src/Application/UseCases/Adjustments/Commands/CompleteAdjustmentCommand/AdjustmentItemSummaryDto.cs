using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;

public class AdjustmentItemSummaryDto
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public Ownership Ownership { get; set; } = null!;
    public int NewQty { get; set; }
    public int OldQty { get; set; }
    public int NewQtyPerPkg { get; set; }
    public int OldQtyPerPkg { get; set; }
    public int TotalDifferent => NewQty - OldQty;
    public int TotalDifferentPkg { get; set; }
    public bool PositiveAdjustment => TotalDifferent > 0 ? true : ((TotalDifferent < 0 || TotalDifferentPkg < 0) ? false : true);

    public InventoryTransactionPerSupplier CreateNewInventoryTransactionPerSupplier(Adjustment adjustment, InventoryTransactionPerSupplier latestTransaction)
    {
        return new InventoryTransactionPerSupplier
        {
            JobNo = adjustment.JobNo,
            ProductCode = ProductCode,
            SupplierId = SupplierId,
            CustomerCode = CustomerCode,
            Ownership = Ownership,
            JobDate = adjustment.CreatedDate,
            Act = PositiveAdjustment ? InventoryTransactionType.PositiveAdjustment : InventoryTransactionType.NegativeAdjustment,
            Qty = Math.Abs(TotalDifferent),
            BalanceQty = TotalDifferent + latestTransaction.BalanceQty
        };
    }
}
