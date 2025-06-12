namespace Domain.Exceptions;

[Serializable]
internal class UnsupportedScannerTypeException : DomainException
{
    public UnsupportedScannerTypeException() { }
    public UnsupportedScannerTypeException(string? message) : base(message) { }
}