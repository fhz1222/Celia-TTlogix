namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedInventoryAdjustmentJobTypeException : DomainException
{
    public UnsupportedInventoryAdjustmentJobTypeException() { }
    public UnsupportedInventoryAdjustmentJobTypeException(string? message) : base(message) { }
}