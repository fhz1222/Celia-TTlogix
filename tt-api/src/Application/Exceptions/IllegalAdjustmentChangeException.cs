namespace Application.Exceptions;

public class IllegalAdjustmentChangeException : TtlogixApiException
{
    public IllegalAdjustmentChangeException() : base() { }
    public IllegalAdjustmentChangeException(string message) : base(message) { }
}
