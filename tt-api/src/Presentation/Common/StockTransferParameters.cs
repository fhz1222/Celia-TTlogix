using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

/// <summary>
/// This class is used by StockTransferController to specify required parameters, filters, sorting and pagination requirements
/// for the Stock Transfer reversal list.
/// </summary>
public class StockTransferParameters
{
    /// <summary>
    /// JobNo
    /// </summary>
    public string? JobNo { get; set; }

    /// <summary>
    /// WHS code
    /// </summary>
    public string? WhsCode { get; set; }

    /// <summary>
    /// Customer code
    /// </summary>
    public string? CustomerCode { get; set; }

    /// <summary>
    /// Reference number filter
    /// </summary>
    public string? RefNo { get; set; }

    /// <summary>
    /// Status filter <br/>
    /// </summary>
    public IEnumerable<int>? Statuses { get; set; }

    /// <summary>
    /// Reason filter
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Created date filter
    /// </summary>
    public DtoFilterDateTimeRange? CreatedDate { get; set; }

    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }
}



