using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

/// <summary>
/// This class is used by StorageDetailItemController to specify required parameters, filters, sorting and pagination requirements
/// for the storage detail item list. CustomerCode and Paging have to be always given and at least one of these three: ProductCode, PID or ExternalPID
/// </summary>
public class StorageDetailItemParameters
{
    /// <summary>
    /// Customer code - mandatory
    /// </summary>
    public string? CustomerCode { get; set; }

    /// <summary>
    /// Supplier identifier filter
    /// </summary>
    public string? SupplierId { get; set; }

    /// <summary>
    /// Product code filter
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Pallet identifier filter
    /// </summary>
    public string? PID { get; set; }

    /// <summary>
    /// External pallet identifier filter
    /// </summary>
    public string? ExternalPID { get; set; }

    /// <summary>
    /// Location filter
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Stock ownership
    /// </summary>
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

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



