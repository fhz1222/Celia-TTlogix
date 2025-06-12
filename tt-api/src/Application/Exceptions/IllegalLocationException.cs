namespace Application.Exceptions;

public class IllegalLocationException : TtlogixApiException
{
    public IllegalLocationException() : base() { }
    public IllegalLocationException(string message) : base(message) { }
}
