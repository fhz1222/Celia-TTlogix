using Application.Exceptions;
using Domain.ValueObjects;

namespace Application.UseCases.Inventory;

public class InventoryItemDto
{
    public Domain.Entities.Product Product { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public Ownership Ownership { get; set; } = null!;
    public int OnHandQty { get; set; }
    public int OnHandPkg { get; set; }
    public int QuarantineQty { get; set; }
    public int QuarantinePkg { get; set; }
    public int AllocatedQty { get; set; }
    public int IncomingQty { get; set; }
    public int PickableQty => OnHandQty - QuarantineQty - AllocatedQty;
    public int BondedQty { get; set; }
    public int NonBondedQty => OnHandQty - BondedQty;

    public static Func<InventoryItemDto, object>? GetOrderByFunction(string? by)
    {
        switch (by?.ToLower())
        {
            case null: return null;
            case "supplierid": return (i) => i.Product.CustomerSupplier.SupplierId;
            case "productcode": return (i) => i.Product.Code;
            case "ownership": return (i) => (byte)i.Ownership;
            case "onhandqty": return (i) => i.OnHandQty;
            case "quarantineqty": return (i) => i.QuarantineQty;
            case "allocatedqty": return (i) => i.AllocatedQty;
            case "incomingqty": return (i) => i.IncomingQty;
            case "pickableqty": return (i) => i.PickableQty;
            case "bondedqty": return (i) => i.BondedQty;
            case "nonbondedqty": return (i) => i.NonBondedQty;
            default: throw new UnknownOrderByExpressionException($"OrderBy expression '{by}' was not recognized");
        }
    }
}