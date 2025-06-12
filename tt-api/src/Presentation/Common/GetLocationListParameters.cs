using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by Registration Controller to specify required parameters, filters, sorting and pagination requirements
/// for the location list.
/// </summary>
public class GetLocationListParameters
{
    /// <summary>
    /// Code
    /// </summary>
    public string? Code { get; set; } = null!;

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Type
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Warehouse Code
    /// </summary>
    public string? WarehouseCode { get; set; }

    /// <summary>
    /// Area code
    /// </summary>
    public string? AreaCode { get; set; }

    /// <summary>
    /// Priority
    /// </summary>
    public int? IsPriority { get; set; }

    /// <summary>
    /// ILog Location Category
    /// </summary>
    public int? ILogLocationCategoryId { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }

    /// <summary>
    /// Pagination specifies page number and size of page; mandatory
    /// </summary>
    public PaginationQuery Pagination { get; set; } = null!;
}



