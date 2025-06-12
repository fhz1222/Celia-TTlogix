using Application.UseCases.InvoiceRequest;

namespace Application.Interfaces.Gateways;

public interface INotificationGateway
{
    Task EmailAboutApprovedInvoiceRequestCustomsFlow(CustomerSupplierDto supplier, JobForSupplier job, List<ProductLineDto> lines, MemoryStream excel, List<NamedStream> invoices, string depHour, string? comment);
    Task EmailAboutApprovedInvoiceBatchStandardFlow(CustomerSupplierDto supplier, List<string> docketNumbers, List<NamedStream> invoices);
    Task EmailAboutBatchWithPriceValidationError(CustomerSupplierDto supplier, List<string> docketNumbers, string ttlogixPrice, string invoicePrice);
    Task EmailAboutInvoiceBatchUploadCustomsFlow(string companyName, List<(string dd, string factory)> details);
    Task EmailAboutInvoiceRequest(CustomerSupplierDto supplier, JobForSupplier job, List<ProductLineDto> lines, MemoryStream excel);
    Task EmailAboutRejectedInvoiceBatch(CustomerSupplierDto supplier, List<string> docketNumbers);
}
