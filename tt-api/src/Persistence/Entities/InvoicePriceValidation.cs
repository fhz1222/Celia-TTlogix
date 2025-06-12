namespace Persistence.Entities;

public class InvoicePriceValidation
{
    public int Id { get; set; }
    public string Currency { get; set; } = null!;
    public decimal InvoiceTotalValue { get; set; }
    public decimal TtlogixTotalValue { get; set; }
    public bool Success { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;
}
