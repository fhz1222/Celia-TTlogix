using Domain.ValueObjects;

namespace Application.UseCases.CompanyProfiles;

public class CompanyProfileDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Status Status { get; set; } = null!;
}