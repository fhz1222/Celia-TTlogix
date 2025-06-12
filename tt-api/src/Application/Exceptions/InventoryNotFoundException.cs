namespace Application.Exceptions;

public class InventoryNotFoundException : TtlogixApiException
{
    public InventoryNotFoundException() : base() { }
    public InventoryNotFoundException(string message) : base(message) { }
}
