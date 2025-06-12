namespace Application.UseCases.InvoiceRequest;

public class ProductLineDto
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string InboundJob { get; set; } = null!;
    public string? Im4No { get; set; } = null!;
    public int Qty { get; set; }
    public int PIDCount { get; set; }
    public string? AsnNo { get; set; }
    public string? PoNumber { get; set; }
    public string? PoLineNo { get; set; }
}
