using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class Status : ValueObject
{
    private int Value { get; set; }

    private Status(int value)
    {
        Value = value;
    }

    public static Status From(int value)
    {
        var status = new Status(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Unsupported status {value}.");
        }
        return status;
    }

    public static Status Active => new(1);
    public static Status Inactive => new(0);

    private static IEnumerable<Status> SupportedStatuses => new[] { Active, Inactive };

    public static implicit operator int(Status status) => status.Value;

    public static explicit operator Status(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
