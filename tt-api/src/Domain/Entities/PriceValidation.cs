namespace Domain.Entities;

public class PriceValidation
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public string Currency { get; set; } = null!;
    public decimal InvoiceTotalValue { get; set; }
    public decimal TtlogixTotalValue { get; set; }
    public bool Success { get; set; }
#pragma warning restore CS8618

    public bool HasFailed => !Success;
}
