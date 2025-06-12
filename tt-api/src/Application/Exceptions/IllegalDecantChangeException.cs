namespace Application.Exceptions;

public class IllegalDecantChangeException : TtlogixApiException
{
    public IllegalDecantChangeException() : base() { }
    public IllegalDecantChangeException(string message) : base(message) { }
}
