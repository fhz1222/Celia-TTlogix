using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Decants.Commands.CompleteDecantCommand;

public class DecantItemSummaryByProductDto
{
    public string WhsCode { get; set; } = null!;
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public DateTime JobDate { get; set; }
    public int TotalPkg { get; set; }
    public int Pkg => TotalPkg - 1;
    public bool PositiveAdjustment => true;

    public InventoryTransaction CreateNewInventoryTransaction(InventoryTransaction? latestTransaction)
    {
        return new InventoryTransaction
        {
            JobNo = JobNo,
            ProductCode = ProductCode,
            CustomerCode = CustomerCode,
            JobDate = JobDate,
            Act = PositiveAdjustment ? InventoryTransactionType.PositiveAdjustment : InventoryTransactionType.NegativeAdjustment,
            Qty = 0,
            Pkg = Pkg,
            BalanceQty = latestTransaction?.BalanceQty ?? 0,
            BalancePkg = Pkg + latestTransaction?.BalancePkg ?? 0
        };
    }

    public InventoryTransactionPerWhsCode CreateNewInventoryTransactionPerWhsCode(InventoryTransactionPerWhsCode? latestTransaction)
    {
        return new InventoryTransactionPerWhsCode
        {
            JobNo = JobNo,
            ProductCode = ProductCode,
            CustomerCode = CustomerCode,
            JobDate = JobDate,
            WhsCode = WhsCode,
            Act = PositiveAdjustment ? InventoryTransactionType.PositiveAdjustment : InventoryTransactionType.NegativeAdjustment,
            Qty = 0,
            Pkg = Pkg,
            BalanceQty = latestTransaction?.BalanceQty ?? 0,
            BalancePkg = Pkg + latestTransaction?.BalancePkg ?? 0
        };
    }
}
