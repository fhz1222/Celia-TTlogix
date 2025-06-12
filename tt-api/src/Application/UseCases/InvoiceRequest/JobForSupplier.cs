namespace Application.UseCases.InvoiceRequest;

public class JobForSupplier
{
    public string JobNo { get; set; } = default!;
    public string SupplierId { get; set; } = default!;
    public string CustomerCode { get; set; } = default!;
    public string? DeliveryDocket { get; set; }
    public DateTime? ETD { get; set; }

    public string SupplierRefNo => DeliveryDocket ?? JobNo;
}
