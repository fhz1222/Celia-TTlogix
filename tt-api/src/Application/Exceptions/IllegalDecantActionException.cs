namespace Application.Exceptions;

public class IllegalDecantActionException : TtlogixApiException
{
    public IllegalDecantActionException() : base() { }
    public IllegalDecantActionException(string message) : base(message) { }
}
