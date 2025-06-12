namespace Application.Exceptions;

public class UnknownPIDException : TtlogixApiException
{
    public UnknownPIDException() : base() { }
    public UnknownPIDException(string message) : base(message) { }
}
