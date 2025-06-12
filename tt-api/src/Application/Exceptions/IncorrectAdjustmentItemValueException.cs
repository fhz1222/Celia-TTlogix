namespace Application.Exceptions;

public class IncorrectAdjustmentItemValueException : TtlogixApiException
{
    public IncorrectAdjustmentItemValueException() : base() { }
    public IncorrectAdjustmentItemValueException(string message) : base(message) { }
}
