using Application.Common.Enums;
using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;
/// <summary>
/// This class is used by DecantController to specify required parameters, filters, sorting and pagination requirements
/// for the decant list. Status and Paging have to be always given 
/// </summary>
public class DecantParameters
{
    /// <summary>
    /// Specifies customer code - optional
    /// </summary>
    public string? CustomerCode { get; set; } = null!;
    /// Specifies jobNo filter - optional <br/>
    /// JobNo is unique indentifier for a decant; it is possible to filter by fragment of the job no text
    /// </summary>
    public string? JobNo { get; set; }
    /// <summary>
    /// <summary>
    /// Specifies reference number filter - optional
    /// </summary>
    public string? ReferenceNo { get; set; }
    /// <summary>
    /// Remark filter - optional<br/>
    /// It matches a fragment or a whole text in the decant remark field
    /// </summary>
    public string? Remark { get; set; }
    /// <summary>
    /// Status filter; default value is Outstanding <br/>
    /// Possible values:<br/> 
    /// - <b>New</b> (0) <br/>
    /// - <b>Processing</b> (1)<br/>
    /// - <b>Outstanding</b> (7 - virtual): returns New and Processing decants<br/>
    /// - <b>Completed</b> (8) <br/>
    /// - <b>Cancelled</b> (9)<br/>
    /// - <b>All</b> (10 - virtual): returns decant objects with any status<br/>
    /// </summary>
    public DecantFilterStatus Status { get; set; } = DecantFilterStatus.Outstanding;
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



