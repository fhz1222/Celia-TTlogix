using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Ownership : ValueObject
{
    private int Value { get; set; }

    private Ownership(int value)
    {
        Value = value;
    }

    public static Ownership From(int value)
    {
        var ownership = new Ownership(value);
        if (!SupportedOwnerships.Contains(ownership))
        {
            throw new UnsupportedOwnershipException();
        }
        return ownership;
    }

    public static Ownership EHP => new(1);
    public static Ownership Supplier => new(0);

    private static IEnumerable<Ownership> SupportedOwnerships => new[] { EHP, Supplier };

    public static implicit operator int(Ownership ownership) => ownership.Value;

    public static explicit operator Ownership(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
