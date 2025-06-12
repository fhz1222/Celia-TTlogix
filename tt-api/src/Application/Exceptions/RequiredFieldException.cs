namespace Application.Exceptions;

public class RequiredFieldException : TtlogixApiException
{
    public RequiredFieldException() : base() { }
    public RequiredFieldException(string message) : base(message) { }
}
