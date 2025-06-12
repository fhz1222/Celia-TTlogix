namespace Domain.Entities;

public class DecantItem
{
    public string SourcePalletId { get; set; } = null!;
    public int SourceQty { get; set; }

    public ICollection<DecantItemPallet> NewPallets { get; set; } = new List<DecantItemPallet>();
}