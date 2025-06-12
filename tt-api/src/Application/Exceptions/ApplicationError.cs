namespace Application.Exceptions;

public class ApplicationError : Exception
{
    public ApplicationError() : base() { }
    public ApplicationError(string message) : base(message) { }
}
