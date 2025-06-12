using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class CustomerClientStatus : ValueObject
{
    private int Value { get; set; }

    private CustomerClientStatus(int value)
    {
        Value = value;
    }

    public static CustomerClientStatus From(int value)
    {
        var status = new CustomerClientStatus(value);
        if (!SupportedCustomerClientStatuses.Contains(status))
        {
            throw new UnsupportedValueException($"Unsupported customer client status {value}.");
        }
        return status;
    }

    public static CustomerClientStatus Active => new(1);
    public static CustomerClientStatus Inactive => new(0);

    private static IEnumerable<CustomerClientStatus> SupportedCustomerClientStatuses => new[] { Active, Inactive };

    public static implicit operator int(CustomerClientStatus status) => status.Value;

    public static explicit operator CustomerClientStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
