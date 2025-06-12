namespace Application.UseCases.InvoiceRequest.Commands.UploadBatch;

public class UploadInvoiceDto
{
    public string InvoiceNumber { get; set; } = default!;
    public decimal Value { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public byte[] Content { get; set; } = default!;
}
