using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

public class GetBatchesRequestParameters
{
    public string FactoryId { get; set; } = default!;
    public string? SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? AsnNo { get; set; }
    public string? PoNo { get; set; }
    public string? DDSTNumber { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    public PaginationQuery Pagination { get; set; } = default!;
}
