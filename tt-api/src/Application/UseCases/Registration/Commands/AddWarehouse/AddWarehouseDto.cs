using Domain.ValueObjects;

namespace Application.UseCases.Registration.Commands.AddWarehouse;

public class AddWarehouseDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Address1 { get; set; } = null!;
    public string Address2 { get; set; } = null!;
    public string Address3 { get; set; } = null!;
    public string Address4 { get; set; } = null!;
    public string PostCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string Pic { get; set; } = null!;
    public string TelNo { get; set; } = null!;
    public string FaxNo { get; set; } = null!;
    public decimal Area { get; set; }
    public DefinedType Type { get; set; } = null!;
}
