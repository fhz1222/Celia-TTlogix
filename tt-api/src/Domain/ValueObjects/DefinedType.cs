using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class DefinedType : ValueObject
{
    private int Value { get; set; }

    private DefinedType(int value)
    {
        Value = value;
    }

    public static DefinedType From(int value)
    {
        var t = new DefinedType(value);
        if (!SupportedDefinedTypes.Contains(t))
        {
            throw new UnsupportedValueException($"Unsupported Type {value}.");
        }
        return t;
    }

    public static DefinedType SystemDefined => new(0);
    public static DefinedType UserDefined => new(1);

    private static IEnumerable<DefinedType> SupportedDefinedTypes => new[] { SystemDefined, UserDefined };

    public static implicit operator int(DefinedType DefinedType) => DefinedType.Value;

    public static explicit operator DefinedType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
