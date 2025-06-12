using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class InventoryAdjustmentStatus : ValueObject
{
    private int Value { get; set; }

    public InventoryAdjustmentStatus(int value)
    {
        Value = value;
    }

    public static InventoryAdjustmentStatus From(int value)
    {
        var status = new InventoryAdjustmentStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedInventoryAdjustmentStatusException();
        }
        return status;
    }

    public static InventoryAdjustmentStatus New => new(0);
    public static InventoryAdjustmentStatus Processing => new(1);
    public static InventoryAdjustmentStatus Completed => new(2);
    public static InventoryAdjustmentStatus Cancelled => new(10);

    private static IEnumerable<InventoryAdjustmentStatus> SupportedStatuses => new[] { New, Processing, Completed, Cancelled };

    public static implicit operator int(InventoryAdjustmentStatus status) => status.Value;

    public static explicit operator InventoryAdjustmentStatus(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
