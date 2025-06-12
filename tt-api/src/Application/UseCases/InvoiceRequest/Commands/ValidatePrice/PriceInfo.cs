namespace Application.UseCases.InvoiceRequest.Commands.ValidatePrice;

public class PriceInfo
{
    public string ProductCode { get; set; } = default!;
    public int Qty { get; set; } = default!;
    public decimal Price { get; set; } = default;
    public string? Currency { get; set; }
}
