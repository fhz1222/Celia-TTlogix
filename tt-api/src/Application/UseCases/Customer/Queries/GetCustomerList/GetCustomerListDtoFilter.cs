using Domain.Enums;

namespace Application.UseCases.Customer.Queries.GetCustomerList;

public class GetCustomerListDtoFilter
{
    public string WhsCode { get; set; } = null!;
    public string? Code { get; set; } = null!;
    public string? Name { get; set; } = null!;
    public string? ContactPerson { get; set; } = null!;
    public string? TelephoneNo { get; set; }
    public string? FaxNo { get; set; }
    public CustomerStatus? Status { get; set; }
}
