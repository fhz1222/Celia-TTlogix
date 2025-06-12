using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class DecantStatus : ValueObject
{
    private int Value { get; set; }

    public DecantStatus(int value)
    {
        Value = value;
    }

    public static DecantStatus From(int value)
    {
        var status = new DecantStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedDecantStatusException();
        }
        return status;
    }

    public static DecantStatus New => new(0);
    public static DecantStatus Processing => new(1);
    public static DecantStatus Completed => new(8);
    public static DecantStatus Cancelled => new(9);

    private static IEnumerable<DecantStatus> SupportedStatuses => new[] { New, Processing, Completed, Cancelled };

    public static implicit operator int(DecantStatus status) => status.Value;

    public static explicit operator DecantStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
