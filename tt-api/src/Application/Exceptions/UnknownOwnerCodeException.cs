namespace Application.Exceptions;

public class UnknownOwnerCodeException : TtlogixApiException
{
    public UnknownOwnerCodeException() : base() { }
    public UnknownOwnerCodeException(string message) : base(message) { }
}
