using Domain.Enums;

namespace Domain.Entities;

public class InventoryTransactionPerWhsCode
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public DateTime JobDate { get; set; }
    public InventoryTransactionType Act { get; set; }
    public int Qty { get; set; }
    public int Pkg { get; set; }
    public int BalanceQty { get; set; }
    public int BalancePkg { get; set; }
}
