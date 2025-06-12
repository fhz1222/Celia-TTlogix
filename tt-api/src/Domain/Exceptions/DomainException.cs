namespace Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException() { }

    protected DomainException(string? message) : base(message) { }

    public string ExceptionMessage { get; set; }
    public string ExceptionCode { get; set; }
}
