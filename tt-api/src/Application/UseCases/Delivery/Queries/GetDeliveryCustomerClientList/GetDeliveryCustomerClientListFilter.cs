using Domain.ValueObjects;

namespace Application.UseCases.Delivery.Queries.GetDeliveryCustomerClientList;

public class GetDeliveryCustomerClientListFilter
{
    public string CustomerCode { get; set; } = null!;
    public Status Status { get; set; } = null!;
}
