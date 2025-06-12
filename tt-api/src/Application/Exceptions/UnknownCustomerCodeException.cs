namespace Application.Exceptions;

public class UnknownCustomerCodeException : TtlogixApiException
{
    public UnknownCustomerCodeException() : base() { }
    public UnknownCustomerCodeException(string message) : base(message) { }
}
