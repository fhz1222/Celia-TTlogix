namespace Domain.Entities;

public class Split
{
    public string JobNo { get; set; } = null!;
    public Pallet SourcePallet { get; set; } = null!;
    public Pallet NewPallet1 { get; set; } = null!;
    public Pallet NewPallet2 { get; set; } = null!;
}