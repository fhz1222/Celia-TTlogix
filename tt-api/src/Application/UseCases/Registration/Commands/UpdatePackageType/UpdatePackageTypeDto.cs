using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.UpdatePackageType;

public class UpdatePackageTypeDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Status Status { get; set; } = null!;
}
