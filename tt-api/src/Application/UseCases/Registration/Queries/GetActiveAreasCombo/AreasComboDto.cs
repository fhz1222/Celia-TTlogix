using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetActiveAreasCombo;

public class AreasComboDto
{
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
}
