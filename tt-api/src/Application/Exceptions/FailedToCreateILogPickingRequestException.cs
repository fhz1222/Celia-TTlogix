namespace Application.Exceptions;

public class FailedToCreateILogPickingRequestException : TtlogixApiException
{
    public FailedToCreateILogPickingRequestException() : base() { }
    public FailedToCreateILogPickingRequestException(string message) : base(message) { }
}
