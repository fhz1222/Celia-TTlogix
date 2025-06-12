
using Domain.ValueObjects;

namespace Application.UseCases.Customer;

public class CustomerClientListItemDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; } = null!;
    public string? TelephoneNo { get; set; }
    public string? FaxNo { get; set; }
    public CustomerClientStatus Status { get; set; } = null!;
}