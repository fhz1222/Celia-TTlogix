namespace Application.Exceptions;

public class IllegalDateFilterException : TtlogixApiException
{
    public IllegalDateFilterException() : base() { }
    public IllegalDateFilterException(string message) : base(message) { }
}
