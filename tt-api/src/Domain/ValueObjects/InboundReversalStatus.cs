using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class InboundReversalStatus : ValueObject
{
    private int Value { get; set; }

    private InboundReversalStatus(int value)
    {
        Value = value;
    }

    public static InboundReversalStatus From(int value)
    {
        var status = new InboundReversalStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid inbound reversal status {value}.");
        }
        return status;
    }

    public static InboundReversalStatus New => new(0);
    public static InboundReversalStatus Processing => new(1);
    public static InboundReversalStatus Completed => new(8);
    public static InboundReversalStatus Cancelled => new(9);

    private static IEnumerable<InboundReversalStatus> SupportedStatuses => new[] { New, Processing, Completed, Cancelled };

    public static implicit operator int(InboundReversalStatus status) => status.Value;

    public static explicit operator InboundReversalStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
