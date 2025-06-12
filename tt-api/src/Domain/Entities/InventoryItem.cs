using Domain.ValueObjects;

namespace Domain.Entities;

public class InventoryItem
{
    public Product Product { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public Ownership Ownership { get; set; } = null!;
    public int OnHandQty { get; set; }
    public int OnHandPkg { get; set; }
    public int QuarantineQty { get; set; }
    public int QuarantinePkg { get; set; }
    public int AllocatedQty { get; set; }
    public int AllocatedPkg { get; set; }
    public int IncomingQty { get; set; }
    public int BondedQty { get; set; }

    public void SetQuarantineValues(int newQty, int oldQty)
    {
        QuarantineQty += newQty - oldQty;
        if (newQty == 0)
            QuarantinePkg -= 1;
    }

    public void SetOnHandValues(int totalDifferent, int totalDifferentPkg)
    {
        OnHandQty += totalDifferent;
        OnHandPkg += totalDifferentPkg;
    }
}
