namespace Application.UseCases.InvoiceRequest;

public class CustomerSupplierDto
{
    public string FactoryId { get; set; } = default!;
    public string FactoryName { get; set; } = default!;
    public string SupplierId { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
}