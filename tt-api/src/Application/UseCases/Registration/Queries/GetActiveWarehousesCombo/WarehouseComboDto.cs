using Domain.ValueObjects;

namespace Application.UseCases.Registration.Queries.GetActiveWarehousesCombo;

public class WarehouseComboDto
{
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
}
