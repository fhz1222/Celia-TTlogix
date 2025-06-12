using Domain.ValueObjects;

namespace Application.UseCases.Customer;

public class UomFilter
{
    public Status Status { get; set; } = null!;
}