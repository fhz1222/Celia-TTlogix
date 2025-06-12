using Domain.ValueObjects;

namespace Application.UseCases.Customer;

public class ControlCodeFilter
{
    public Status Status { get; set; } = null!;
}