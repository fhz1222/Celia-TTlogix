using Domain.ValueObjects;

namespace Domain.Entities;

public class Uom
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
}
