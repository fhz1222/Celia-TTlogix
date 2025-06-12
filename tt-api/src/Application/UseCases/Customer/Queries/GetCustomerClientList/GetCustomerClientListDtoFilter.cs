using Domain.ValueObjects;

namespace Application.UseCases.Customer.Queries.GetCustomerClientList;

public class GetCustomerClientListDtoFilter
{
    public string CustomerCode { get; set; } = null!;
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? ContactPerson { get; set; } = null!;
    public string? TelephoneNo { get; set; } = null!;
    public string? FaxNo { get; set; } = null!;
    public CustomerClientStatus? Status { get; set; }
}
