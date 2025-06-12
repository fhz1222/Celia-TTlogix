namespace Application.Exceptions;

public class TransactionNotFoundException : TtlogixApiException
{
    public TransactionNotFoundException() : base() { }
    public TransactionNotFoundException(string message) : base(message) { }
}
