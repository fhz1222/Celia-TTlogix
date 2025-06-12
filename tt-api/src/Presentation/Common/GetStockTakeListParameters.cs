using Application.Common.Models;
using Application.UseCases;

namespace Presentation.Common;

/// <summary>
/// This class is used by StockTakeController to specify required parameters, filters, sorting and pagination requirements
/// for the stock take list.
/// </summary>
public class GetStockTakeListParameters
{
    /// <summary>
    /// JobNo
    /// </summary>
    public string? JobNo { get; set; }

    /// <summary>
    /// Reference number filter
    /// </summary>
    public string? RefNo { get; set; }

    /// <summary>
    /// WHS code - mandatory
    /// </summary>
    public string WhsCode { get; set; } = null!;

    /// <summary>
    /// Location code
    /// </summary>
    public string? LocationCode { get; set; }

    /// <summary>
    /// Created date
    /// </summary>
    public DtoFilterDateTimeRange? CreatedDate { get; set; }

    /// <summary>
    /// Status filter
    /// </summary>
    public IEnumerable<int>? Statuses { get; set; }

    /// <summary>
    /// Remark filter
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



