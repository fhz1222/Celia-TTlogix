using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class OutboundStatus : ValueObject
{
    private int Value { get; set; }

    private OutboundStatus(int value)
    {
        Value = value;
    }

    public static OutboundStatus From(int value)
    {
        var status = new OutboundStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid outbound status {value}.");
        }
        return status;
    }

    public static OutboundStatus New => new(0);
    public static OutboundStatus PartialPicked => new(3);
    public static OutboundStatus Picked => new(4);
    public static OutboundStatus Packed => new(5);
    public static OutboundStatus InTransit => new(7);
    public static OutboundStatus Completed => new(8);
    public static OutboundStatus Cancelled => new(9);

    private static IEnumerable<OutboundStatus> SupportedStatuses
        => new[] { New, PartialPicked, Picked, Packed, InTransit, Completed, Cancelled };

    public static implicit operator int(OutboundStatus type) => type.Value;

    public static implicit operator byte(OutboundStatus type) => (byte)type.Value;

    public static explicit operator OutboundStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
