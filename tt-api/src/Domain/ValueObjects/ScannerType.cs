using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class ScannerType : ValueObject
{
    private int Value { get; set; }

    private ScannerType(int value)
    {
        Value = value;
    }

    public static ScannerType From(int value)
    {
        var scannerType = new ScannerType(value);
        if (!SupportedTypes.Contains(scannerType))
        {
            throw new UnsupportedScannerTypeException();
        }
        return scannerType;
    }

    public static ScannerType BatchScanner => new(0);
    public static ScannerType RFScanner => new(1);
    public static ScannerType ILogScanner => new(2);

    private static IEnumerable<ScannerType> SupportedTypes => new[] { BatchScanner, RFScanner, ILogScanner };

    public static implicit operator int(ScannerType t) => t.Value;

    public static explicit operator ScannerType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
