using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class InboundType : ValueObject
{
    private int Value { get; set; }

    private InboundType(int value)
    {
        Value = value;
    }

    public static InboundType From(int value)
    {
        var status = new InboundType(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid inbound type {value}.");
        }
        return status;
    }

    public static InboundType ManualEntry => new(0);
    public static InboundType ASN => new(1);
    public static InboundType Return => new(2);
    public static InboundType Excess => new(3);
    public static InboundType ScannerManual => new(4);

    private static IEnumerable<InboundType> SupportedStatuses => new[] { ManualEntry, ASN, Return, Excess, ScannerManual };

    public static implicit operator int(InboundType status) => status.Value;

    public static explicit operator InboundType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
