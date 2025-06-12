using Domain.ValueObjects;

namespace Application.UseCases.Registration;

public class LocationListItemDto
{
    public string Code { get; set; } = null!;
    public string WarehouseCode { get; set; } = null!;
    public string AreaCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public decimal M3 { get; set; }
    public Status Status { get; set; } = null!;
    public LocationType Type { get; set; } = null!;
    public int? IsPriority { get; set; }
    public int ILogLocationCategoryId { get; set; }
}
