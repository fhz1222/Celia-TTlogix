namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedDecantStatusException : DomainException
{
    public UnsupportedDecantStatusException() { }
    public UnsupportedDecantStatusException(string? message) : base(message) { }
}