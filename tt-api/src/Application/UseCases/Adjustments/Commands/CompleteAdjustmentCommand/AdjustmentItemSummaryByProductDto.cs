using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Adjustments.Commands.CompleteAdjustmentCommand;

public class AdjustmentItemSummaryByProductDto
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public int NewQty { get; set; }
    public int OldQty { get; set; }
    public int NewQtyPerPkg { get; set; }
    public int OldQtyPerPkg { get; set; }
    public int TotalDifferent => NewQty - OldQty;
    public int TotalDifferentPkg { get; set; }
    public bool PositiveAdjustment => TotalDifferent > 0 ? true : ((TotalDifferent < 0 || TotalDifferentPkg < 0) ? false : true);

    public InventoryTransaction CreateNewInventoryTransaction(Adjustment adjustment, InventoryTransaction? latestTransaction)
    {
        return new InventoryTransaction
        {
            JobNo = adjustment.JobNo,
            ProductCode = ProductCode,
            CustomerCode = CustomerCode,
            JobDate = adjustment.CreatedDate,
            Act = PositiveAdjustment ? InventoryTransactionType.PositiveAdjustment : InventoryTransactionType.NegativeAdjustment,
            Qty = Math.Abs(TotalDifferent),
            Pkg = Math.Abs(TotalDifferentPkg),
            BalanceQty = TotalDifferent + latestTransaction?.BalanceQty ?? 0,
            BalancePkg = TotalDifferentPkg + latestTransaction?.BalancePkg ?? 0
        };
    }

    public InventoryTransactionPerWhsCode CreateNewInventoryTransactionPerWhsCode(Adjustment adjustment, InventoryTransactionPerWhsCode? latestTransaction)
    {
        return new InventoryTransactionPerWhsCode
        {
            JobNo = adjustment.JobNo,
            ProductCode = ProductCode,
            CustomerCode = CustomerCode,
            WhsCode = adjustment.WhsCode,
            JobDate = adjustment.CreatedDate,
            Act = PositiveAdjustment ? InventoryTransactionType.PositiveAdjustment : InventoryTransactionType.NegativeAdjustment,
            Qty = Math.Abs(TotalDifferent),
            Pkg = Math.Abs(TotalDifferentPkg),
            BalanceQty = TotalDifferent + latestTransaction?.BalanceQty ?? 0,
            BalancePkg = TotalDifferentPkg + latestTransaction?.BalancePkg ?? 0
        };
    }
}
