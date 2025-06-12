namespace Application.Exceptions;

public class UnknownUserCodeException : TtlogixApiException
{
    public UnknownUserCodeException() : base() { }
    public UnknownUserCodeException(string message) : base(message) { }
}
