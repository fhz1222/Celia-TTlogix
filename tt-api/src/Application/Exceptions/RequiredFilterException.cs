namespace Application.Exceptions;

public class RequiredFilterException : TtlogixApiException
{
    public RequiredFilterException() : base() { }
    public RequiredFilterException(string message) : base(message) { }
}
