using Domain.ValueObjects;

namespace Application.UseCases.Customer;

public class ProductCodeFilter
{
    public Status Status { get; set; } = null!;
}