namespace Persistence.Entities;

public class InvoiceRequestProduct
{
    public int Id { get; set; }
    public int InvoiceRequestId { get; set; }
    public string ProductCode { get; set; } = null!;
    public string InboundJob { get; set; } = null!;
    public int Qty { get; set; }
    public string? AsnNo { get; set; }
    public string? PoNumber { get; set; }
    public string? PoLineNo { get; set; }
    public int PIDCount { get; set; }

}
