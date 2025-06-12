namespace Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;

public class DeliveryCustomerClientDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
}
