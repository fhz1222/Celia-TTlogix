using Domain.ValueObjects;

namespace Domain.Entities;

public class Warehouse
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
    public string? Pic { get; set; }
    public string? TelNo { get; set; }
    public string? FaxNo { get; set; }
    public decimal Area { get; set; }
    public DefinedType Type { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
}
