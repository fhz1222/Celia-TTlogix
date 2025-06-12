namespace Domain.Exceptions;

public class IncorrectStorageDetailStatusException : DomainException
{
    public IncorrectStorageDetailStatusException() : base() { }
    public IncorrectStorageDetailStatusException(string message) : base(message) { }
}
