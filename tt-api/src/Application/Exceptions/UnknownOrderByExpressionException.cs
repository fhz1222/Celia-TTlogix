namespace Application.Exceptions;

public class UnknownOrderByExpressionException : TtlogixApiException
{
    public UnknownOrderByExpressionException() : base() { }
    public UnknownOrderByExpressionException(string message) : base(message) { }
}
