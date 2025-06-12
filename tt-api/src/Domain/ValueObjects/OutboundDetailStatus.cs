using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class OutboundDetailStatus : ValueObject
{
    private int Value { get; set; }

    private OutboundDetailStatus(int value)
    {
        Value = value;
    }

    public static OutboundDetailStatus From(int value)
    {
        var status = new OutboundDetailStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid outbound detail status {value}.");
        }
        return status;
    }

    public static OutboundDetailStatus New => new(0);
    public static OutboundDetailStatus Picking => new(1);
    public static OutboundDetailStatus Picked => new(2);

    private static IEnumerable<OutboundDetailStatus> SupportedStatuses => new[] { New, Picking, Picked };

    public static implicit operator byte(OutboundDetailStatus type) => (byte)type.Value;
    public static explicit operator OutboundDetailStatus(byte value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}