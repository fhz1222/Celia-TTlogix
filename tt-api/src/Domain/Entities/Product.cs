namespace Domain.Entities;

public class Product
{
#pragma warning disable CS8618
    public string Code { get; set; } = null!;
    public CustomerSupplier CustomerSupplier { get; set; } = null!;
    public bool IsCPart { get; set; }
    public int SPQ { get; set; }
    public int CPartBoxQty { get; set; }
#pragma warning restore CS8618
}
