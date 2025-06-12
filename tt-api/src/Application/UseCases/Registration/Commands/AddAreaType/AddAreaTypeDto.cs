using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddAreaType;

public class AddAreaTypeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
}
