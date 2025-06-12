using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

public class StorageDetailItemWithPartInfoParameters
{
    /// <summary>
    /// Out job no - required
    /// </summary>
    public string? OutboundJobNo { get; set; }
    /// <summary>
    /// line item - required
    /// </summary>
    public int LineItem { get; set; }
    /// <summary>
    /// Pallet identifier filter
    /// </summary>
    public string? PID { get; set; }
    /// <summary>
    /// Inbound Date range
    /// </summary>
    public DateTime? InboundDate { get; set; }
    /// <summary>
    /// Location filter
    /// </summary>
    public string? Location { get; set; }  
    /// <summary>
    /// GroupID
    /// </summary>
    public string? GroupID { get; set; }
    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    /// <summary>
    /// sorting parameters, By can be set to the following values:
    /// PID
    /// Qty
    /// InboundDate
    /// Ownership
    /// LocationCode
    /// GroupID
    /// 
    /// default sorting is by PID asc
    /// </summary>
    public OrderBy? Sorting { get; set; }

}
