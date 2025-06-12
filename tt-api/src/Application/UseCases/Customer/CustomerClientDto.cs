namespace Application.UseCases.Customer;

public class CustomerClientDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public string PrimaryAddress { get; set; } = null!;
    public string BillingAddress { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public string Pic1 { get; set; } = null!;
    public string Pic2 { get; set; } = null!;
}