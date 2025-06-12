using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class LocationType : ValueObject
{
    private int Value { get; set; }

    private LocationType(int value)
    {
        Value = value;
    }

    public static LocationType From(int value)
    {
        var status = new LocationType(value);
        if (!SupportedTypes.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid location type {value}.");
        }
        return status;
    }

    public static LocationType Normal => new(0);
    public static LocationType Quarantine => new(1);
    public static LocationType CrossDock => new(2);
    public static LocationType Standby => new(3);
    public static LocationType ExtSystem => new(4);

    private static IEnumerable<LocationType> SupportedTypes => new[] { Normal, Quarantine, CrossDock, Standby, ExtSystem };

    public static implicit operator int(LocationType type) => type.Value;

    public static explicit operator LocationType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}