using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

// TODO why both value object and enum for pallet status?
public class StorageStatus : ValueObject
{
    private int Value { get; set; }

    private StorageStatus(int value)
    {
        Value = value;
    }

    public static StorageStatus From(int value)
    {
        var status = new StorageStatus(value);
        if (!SupportedStatuses.Contains(status))
        {
            throw new UnsupportedStorageStatusException();
        }
        return status;
    }

    public static StorageStatus Incoming => new(0);
    public static StorageStatus Putaway => new(1);
    public static StorageStatus Allocated => new(2);
    public static StorageStatus Picked => new(3);
    public static StorageStatus Packed => new(4);
    public static StorageStatus InTransit => new(5);
    public static StorageStatus Dispatched => new(6);
    public static StorageStatus Splitted => new(7);
    public static StorageStatus Quarantine => new(9);

    //Temporary status before Allocated
    public static StorageStatus Allocating => new(10);
    public static StorageStatus Kitting => new(11);
    public static StorageStatus Splitting => new(12);

    public static StorageStatus Restricted => new(13);

    //Status 21 - 30 is reserved for item locked by warehouse operation
    public static StorageStatus Transferring => new(21);
    public static StorageStatus Decant => new(22);
    public static StorageStatus Discrepancy => new(23);

    public static StorageStatus DiscrepancyFixed => new(96);
    public static StorageStatus Reversal => new(97);
    public static StorageStatus ZeroOut => new(98);
    public static StorageStatus Cancelled => new(99);

    private static IEnumerable<StorageStatus> SupportedStatuses => new[] {Incoming, Putaway, Allocated, Picked, Packed, InTransit, Dispatched, Splitted, Quarantine
        , Allocating, Kitting, Splitting, Transferring, Decant, Discrepancy, DiscrepancyFixed, Reversal, ZeroOut, Cancelled, Restricted};

    public static implicit operator int(StorageStatus status) => status.Value;

    public static explicit operator StorageStatus(int value) => From(value);
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
