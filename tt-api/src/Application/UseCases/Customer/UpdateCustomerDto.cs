namespace Application.UseCases.Customer;

public class UpdateCustomerDto
{
    public string Code { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CompanyCode { get; set; } = null!;
    public string BizUnit { get; set; } = null!;
    public string Buname { get; set; } = null!;
    public string PrimaryAddress { get; set; } = null!;
    public string BillingAddress { get; set; } = null!;
    public string ShippingAddress { get; set; } = null!;
    public string Pic1 { get; set; } = null!;
    public string Pic2 { get; set; } = null!;
}