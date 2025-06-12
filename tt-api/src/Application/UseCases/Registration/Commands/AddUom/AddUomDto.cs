using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddUom;

public class AddUomDto
{
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
}
