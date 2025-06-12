using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class JobStatus : ValueObject
{
    private int Value { get; set; }

    private JobStatus(int value)
    {
        Value = value;
    }

    public static JobStatus From(int value)
    {
        var status = new JobStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Unsupported job status {value}.");
        }
        return status;
    }

    public static JobStatus New => new(0);
    public static JobStatus Processing => new(1);
    public static JobStatus Completed => new(8);
    public static JobStatus Cancelled => new(9);

    private static IEnumerable<JobStatus> SupportedStatuses => new[] { New, Processing, Completed, Cancelled };

    public static implicit operator int(JobStatus status) => status.Value;

    public static explicit operator JobStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
