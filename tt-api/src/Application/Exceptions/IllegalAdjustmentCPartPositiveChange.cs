namespace Application.Exceptions;

public class IllegalAdjustmentCPartPositiveChange : TtlogixApiException
{
    public IllegalAdjustmentCPartPositiveChange() : base() { }
    public IllegalAdjustmentCPartPositiveChange(string message) : base(message) { }
}
