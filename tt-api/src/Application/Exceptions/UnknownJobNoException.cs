namespace Application.Exceptions;

public class UnknownJobNoException : TtlogixApiException
{
    public UnknownJobNoException() : base() { }
    public UnknownJobNoException(string message) : base(message) { }
}
