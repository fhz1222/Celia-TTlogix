namespace Application.Exceptions;

public class PalletPickerError : ApplicationError
{
    public PalletPickerError(string message) : base($"Unable to pick: {message}") { }
}