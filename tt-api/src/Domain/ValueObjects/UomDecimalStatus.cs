using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class UomDecimalStatus : ValueObject
{
    private int Value { get; set; }

    private UomDecimalStatus(int value)
    {
        Value = value;
    }

    public static UomDecimalStatus From(int value)
    {
        var status = new UomDecimalStatus(value);
        if (!SupportedUomDecimalStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Unsupported uom decimal status {value}.");
        }
        return status;
    }

    public static UomDecimalStatus Active => new(1);
    public static UomDecimalStatus Inactive => new(0);

    private static IEnumerable<UomDecimalStatus> SupportedUomDecimalStatuses => new[] { Active, Inactive };

    public static implicit operator int(UomDecimalStatus status) => status.Value;

    public static explicit operator UomDecimalStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
