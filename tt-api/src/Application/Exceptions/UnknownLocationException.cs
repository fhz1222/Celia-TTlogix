namespace Application.Exceptions;

public class UnknownLocationException : TtlogixApiException
{
    public UnknownLocationException() : base() { }
    public UnknownLocationException(string message) : base(message) { }
}
