namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedStorageStatusException : DomainException
{
    public UnsupportedStorageStatusException() { }
    public UnsupportedStorageStatusException(string? message) : base(message) { }
}