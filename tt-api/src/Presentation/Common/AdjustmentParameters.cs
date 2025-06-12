using Application.Common.Enums;
using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;
/// <summary>
/// This class is used by AdjustmentController to specify required parameters, filters, sorting and pagination requirements
/// for the adjustment list. Status and Paging have to be always given 
/// </summary>
public class AdjustmentParameters
{
    /// <summary>
    /// Specifies customer code - optional
    /// </summary>
    public string? CustomerCode { get; set; } = null!;
    /// <summary>
    /// Specifies jobNo filter - optional <br/>
    /// JobNo is unique indentifier for an adjustment; it is possible to filter by fragment of the job no text
    /// </summary>
    public string? JobNo { get; set; }
    /// <summary>
    /// Specifies reference number filter - optional
    /// </summary>
    public string? ReferenceNo { get; set; }
    /// <summary>
    /// Reason filter - optional<br/>
    /// It matches a fragment or a whole text in the adjustment reason field
    /// </summary>
    public string? Reason { get; set; }
    /// <summary>
    /// Status filter; default value is Outstanding <br/>
    /// Possible values:<br/> 
    /// - <b>New</b> (0) <br/>
    /// - <b>Processing</b> (1)<br/>
    /// - <b>Completed</b> (2) <br/>
    /// - <b>Outstanding</b> (3 - virtual): returns New and Processing adjustments<br/>
    /// - <b>Cancelled</b> (10)<br/>
    /// - <b>All</b> (11 - virtual): returns adjustment with any status<br/>
    /// </summary>
    public AdjustmentFilterStatus Status { get; set; } = AdjustmentFilterStatus.Outstanding;
    /// <summary>
    /// Job type filter <br/>
    /// Possible values: <br/>
    /// - <b>Normal</b> <br/>
    /// - <b>UndoZeroOut </b> 
    /// </summary>
    public int? JobType { get; set; }
    /// <summary>
    /// Created date filter - it is possible to specify <b>date from</b>, <b>date to</b> or <b>range of dates</b> to filter adjustments by creation date
    /// </summary>
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    /// <summary>
    /// Specifies requested page number and size of the page
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



