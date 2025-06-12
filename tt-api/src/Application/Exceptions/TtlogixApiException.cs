namespace Application.Exceptions;

public abstract class TtlogixApiException : Exception
{
    protected TtlogixApiException() { }
    protected TtlogixApiException(string? message) : base(message) { }

    public string ExceptionMessage { get; set; }
    public string ExceptionCode { get; set; }
}
