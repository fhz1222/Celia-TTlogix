namespace Domain.Exceptions;

public class IncorrectPalletException : DomainException
{
    public IncorrectPalletException() : base() { }
    public IncorrectPalletException(string message) : base(message) { }
}
