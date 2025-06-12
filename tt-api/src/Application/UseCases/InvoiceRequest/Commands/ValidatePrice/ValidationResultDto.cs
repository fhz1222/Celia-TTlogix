namespace Application.UseCases.InvoiceRequest.Commands.ValidatePrice;

public class ValidationResultDto
{
    public bool IsSuccess { get; set; }
    public int PriceValidationId { get; set; }
    public string? ResultText { get; set; }
}
