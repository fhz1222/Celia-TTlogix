using Application.UseCases.InvoiceRequest;
using Application.UseCases.InvoiceRequest.Queries;
using Application.UseCases.InvoiceRequest.Queries.GetBatches;

namespace Application.Interfaces.Gateways;

public interface IExcelWriter
{
    MemoryStream GetBatchesExcel(List<InvoiceBatchDto> data);
    MemoryStream GetInvoiceRequestExcel(CustomerSupplierDto supplier, string jobNo, string supplierRefNo, List<ProductLineDto> lines);
    MemoryStream GetNotInvoicedExcel(List<JobDto> data);
}
