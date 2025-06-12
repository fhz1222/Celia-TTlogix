namespace Application.Exceptions;

public class UnknownWhsCodeException : TtlogixApiException
{
    public UnknownWhsCodeException() : base() { }
    public UnknownWhsCodeException(string message) : base(message) { }
}
