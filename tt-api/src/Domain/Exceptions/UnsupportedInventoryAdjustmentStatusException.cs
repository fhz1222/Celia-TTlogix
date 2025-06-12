namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedInventoryAdjustmentStatusException : DomainException
{
    public UnsupportedInventoryAdjustmentStatusException() { }
    public UnsupportedInventoryAdjustmentStatusException(string? message) : base(message) { }
}