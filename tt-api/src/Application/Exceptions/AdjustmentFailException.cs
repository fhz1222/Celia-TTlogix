namespace Application.Exceptions;

public class AdjustmentFailException : TtlogixApiException
{
    public AdjustmentFailException() : base() { }
    public AdjustmentFailException(string message) : base(message) { }
}
