namespace Application.Exceptions;

public class JobAlreadyCompletedException : TtlogixApiException
{
    public JobAlreadyCompletedException() : base() { }
    public JobAlreadyCompletedException(string message) : base(message) { }
}
