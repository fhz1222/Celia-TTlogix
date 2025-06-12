using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddControlCode;

public class AddControlCodeDto
{
    public string Name { get; set; } = null!;
    public DefinedType Type { get; set; } = null!;
}
