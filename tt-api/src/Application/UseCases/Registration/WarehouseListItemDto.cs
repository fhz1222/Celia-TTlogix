using Domain.ValueObjects;

namespace Application.UseCases.Registration;

public class WarehouseListItemDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
    public Status Status { get; set; } = null!;
}
