using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class InboundStatus : ValueObject
{
    private int Value { get; set; }

    private InboundStatus(int value)
    {
        Value = value;
    }

    public static InboundStatus From(int value)
    {
        var status = new InboundStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid inbound status {value}.");
        }
        return status;
    }

    public static InboundStatus New => new(0);
    public static InboundStatus PartialPutaway => new(3);
    public static InboundStatus Completed => new(8);
    public static InboundStatus Cancelled => new(9);

    private static IEnumerable<InboundStatus> SupportedStatuses => new[] { New, PartialPutaway, Completed, Cancelled };

    public static implicit operator int(InboundStatus status) => status.Value;

    public static explicit operator InboundStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
