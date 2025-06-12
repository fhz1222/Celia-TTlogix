using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class StockTakeRelocationLogType : ValueObject
{
    private int Value { get; set; }

    private StockTakeRelocationLogType(int value)
    {
        Value = value;
    }

    public static StockTakeRelocationLogType From(int value)
    {
        var status = new StockTakeRelocationLogType(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Unsupported stock take relocation log type {value}.");
        }
        return status;
    }

    public static StockTakeRelocationLogType FixExtra => new(0);
    public static StockTakeRelocationLogType FixMissing => new(1);

    private static IEnumerable<StockTakeRelocationLogType> SupportedStatuses => new[] { FixExtra, FixMissing };

    public static implicit operator int(StockTakeRelocationLogType status) => status.Value;

    public static explicit operator StockTakeRelocationLogType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
