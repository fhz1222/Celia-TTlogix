namespace Domain.Entities;

public class Quarantine
{
    public string JobNo { get; set; } = null!;

    public ICollection<QuarantineItem> Items { get; set; } = null!;
}
