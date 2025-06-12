using Domain.ValueObjects;

namespace Domain.Entities;

public class UomDecimal
{
    public string CustomerCode { get; set; } = null!;
    public string UOM { get; set; } = null!;
    public int DecimalNum { get; set; }
    public UomDecimalStatus Status { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? CancelledBy { get; set; }
    public DateTime? CancelledDate { get; set; }
}
