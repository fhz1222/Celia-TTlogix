namespace Persistence.Entities;

public class InvoiceFile
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}
