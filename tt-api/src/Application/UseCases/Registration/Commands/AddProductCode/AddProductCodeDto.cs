using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddProductCode;

public class AddProductCodeDto
{
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
}
