using Domain.ValueObjects;

namespace Domain.Entities;

public class Location
{
#pragma warning disable CS8618
    public string Code { get; set; } = null!;
    public string WarehouseCode { get; set; } = null!;
    public string Name { get; set; } = null!;
    public LocationType Type { get; set; }
    public bool IsActive { get; set; }
#pragma warning restore CS8618

    public bool IsInactive => !IsActive;
}