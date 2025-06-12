namespace Domain.Exceptions;

internal class UnsupportedValueException : DomainException
{
    internal UnsupportedValueException(string message) : base(message) { }
}
