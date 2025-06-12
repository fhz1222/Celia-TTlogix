using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddPackageType;

public class AddPackageTypeDto
{
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
}
