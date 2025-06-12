namespace Persistence.Entities;

public class InvoiceRequestBlocklist
{
    public string JobNo { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = null!;
}
