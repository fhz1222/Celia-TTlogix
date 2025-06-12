
using Domain.Enums;

namespace Application.UseCases.Customer;

public class CustomerDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public CustomerStatus Status { get; set; }
    public string Whscode { get; set; } = null!;
}