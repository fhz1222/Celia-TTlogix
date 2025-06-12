using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class InventoryTransactionPerSupplier
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public Ownership Ownership { get; set; }
    public DateTime JobDate { get; set; }
    public InventoryTransactionType Act { get; set; }
    public int Qty { get; set; }
    public int BalanceQty { get; set; }
}
