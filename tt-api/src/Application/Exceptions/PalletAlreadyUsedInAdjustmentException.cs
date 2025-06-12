namespace Application.Exceptions;

public class PalletAlreadyUsedInAdjustmentException : TtlogixApiException
{
    public PalletAlreadyUsedInAdjustmentException() : base() { }
    public PalletAlreadyUsedInAdjustmentException(string message) : base(message) { }
}
