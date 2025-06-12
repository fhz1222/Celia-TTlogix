using Application.Common.Models;

namespace Application.UseCases.Storage.Queries.GetStorageDetailItems;

public class StorageDetailItemDtoFilter
{
    public string? CustomerCode { get; set; }
    public string? SupplierId { get; set; }
    public string? ProductCode { get; set; }
    public string? PID { get; set; }
    public string? ExternalPID { get; set; }
    public string? Location { get; set; }
    public string WhsCode { get; set; } = null!;
    public int? Ownership { get; set; }
    public string? InboundJobNo { get; set; }
    public DtoFilterIntRange? LineItem { get; set; }
    public DtoFilterIntRange? SequenceNo { get; set; }
    public DtoFilterIntRange? Qty { get; set; }
    public DtoFilterIntRange? AllocatedQty { get; set; }
    public DtoFilterDateTimeRange? InboundDate { get; set; }
    public int? Status { get; set; }
    public string? OutboundJobNo { get; set; }
    public string? ParentId { get; set; }
    public string? RefNo { get; set; }
    public bool? BondedStatus { get; set; }
    public string? CommInvNo { get; set; }

    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    public OrderBy? Sorting { get; set; }
}
