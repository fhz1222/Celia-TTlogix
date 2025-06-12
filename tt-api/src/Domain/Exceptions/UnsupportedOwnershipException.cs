namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedOwnershipException : DomainException
{
    public UnsupportedOwnershipException() { }
    public UnsupportedOwnershipException(string? message) : base(message) { }
}