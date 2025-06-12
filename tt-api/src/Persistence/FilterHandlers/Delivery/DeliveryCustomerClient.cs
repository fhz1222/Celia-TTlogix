using Domain.ValueObjects;

namespace Persistence.FilterHandlers.Delivery;

class DeliveryCustomerClient
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public byte Status { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }
}
