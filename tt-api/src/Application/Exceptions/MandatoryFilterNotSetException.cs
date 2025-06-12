namespace Application.Exceptions;

public class MandatoryFilterNotSetException : TtlogixApiException
{
    public MandatoryFilterNotSetException() : base() { }
    public MandatoryFilterNotSetException(string message) : base(message) { }
}
