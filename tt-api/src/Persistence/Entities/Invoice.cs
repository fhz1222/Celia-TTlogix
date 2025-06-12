namespace Persistence.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int InvoiceBatchId { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public decimal Value { get; set; }
    public string Currency { get; set; } = null!;
}
