using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

public class InventoryAdjustmentJobType : ValueObject
{
    private int Value { get; set; }

    public InventoryAdjustmentJobType(int value)
    {
        Value = value;
    }

    public static InventoryAdjustmentJobType From(int value)
    {
        var jobType = new InventoryAdjustmentJobType(value);
        if (!SupportedJobTypes.Contains(jobType))
        {
            throw new UnsupportedInventoryAdjustmentJobTypeException();
        }
        return jobType;
    }

    public static InventoryAdjustmentJobType Normal => new(0);
    public static InventoryAdjustmentJobType UndoZeroOut => new(1);

    private static IEnumerable<InventoryAdjustmentJobType> SupportedJobTypes => new[] { Normal, UndoZeroOut };

    public static implicit operator int(InventoryAdjustmentJobType jobType) => jobType.Value;

    public static explicit operator InventoryAdjustmentJobType(int value) => From(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
