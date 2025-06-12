using Domain.ValueObjects;

namespace Application.UseCases.Inventory.Queries.GetInventoryItems;

public class InventoryItemDtoFilter
{
    public Ownership? Ownership { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string? SupplierId { get; set; }
    public string? ProductCode { get; set; }
    public string WhsCode { get; set; } = null!;
    public DtoFilterIntRange OnHandQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange QuarantineQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange AllocatedQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange IncomingQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange PickableQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange BondedQty { get; set; } = new DtoFilterIntRange();
    public DtoFilterIntRange NonBondedQty { get; set; } = new DtoFilterIntRange();
}