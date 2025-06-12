using Domain.ValueObjects;

namespace Domain.Entities;

public class Area
{
    public string Code { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal Capacity { get; set; }
    public Status Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
}
