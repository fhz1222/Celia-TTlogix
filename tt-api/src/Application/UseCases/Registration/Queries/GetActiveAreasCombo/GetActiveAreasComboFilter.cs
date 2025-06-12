using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetActiveAreasCombo;

public class GetActiveAreasComboFilter
{
    public string WhsCode { get; set; } = null!;
    public int Status { get; set; }
}
