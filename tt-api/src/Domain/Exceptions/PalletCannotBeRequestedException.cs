namespace Domain.Exceptions;

public class PalletCannotBeRequestedException : DomainException
{
    public PalletCannotBeRequestedException() : base() { }
    public PalletCannotBeRequestedException(string message) : base(message) { }
}
