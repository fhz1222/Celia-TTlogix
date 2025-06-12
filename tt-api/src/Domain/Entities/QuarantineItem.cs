namespace Domain.Entities;

public class QuarantineItem
{
    public string PalletId { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public Pallet Pallet { get; set; } = null!;
}
