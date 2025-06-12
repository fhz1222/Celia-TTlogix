using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetAreaTypesCombo;

public class AreaTypesComboDto
{
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
}
