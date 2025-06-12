using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class OutboundType : ValueObject
{
    private int Value { get; set; }

    private OutboundType(int value)
    {
        Value = value;
    }

    public static OutboundType From(int value)
    {
        var status = new OutboundType(value);
        if (!SupportedTypes.Contains(status))
        {
            throw new UnsupportedValueException($"Invalid outbound type {value}.");
        }
        return status;
    }

    public static OutboundType Manual => new(0);
    public static OutboundType CrossDock => new(1);
    public static OutboundType EKanban => new(2);
    public static OutboundType Return => new(3);
    public static OutboundType WHSTransfer => new(4);
    public static OutboundType WHSTransferScanner => new(5);
    public static OutboundType ScannerManual => new(7);

    private static IEnumerable<OutboundType> SupportedTypes
        => new[] { Manual, CrossDock, EKanban, Return, WHSTransfer, WHSTransferScanner, ScannerManual };

    public static implicit operator int(OutboundType type) => type.Value;

    public static explicit operator OutboundType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}