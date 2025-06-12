namespace Application.Exceptions;

public class IllegalPalletLocationException : TtlogixApiException
{
    public IllegalPalletLocationException() : base() { }
    public IllegalPalletLocationException(string message) : base(message) { }
}
