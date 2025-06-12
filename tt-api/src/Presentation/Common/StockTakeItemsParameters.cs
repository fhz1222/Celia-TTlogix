using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by StockTakeController to specify required parameters and sorting requirements
/// for the Stock Take item list.
/// </summary>
public class StockTakeItemsParameters
{
    /// <summary>
    /// Stock take JobNo
    /// </summary>
    public string JobNo { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending
    /// </summary>  
    public OrderBy? Sorting { get; set; }
}



