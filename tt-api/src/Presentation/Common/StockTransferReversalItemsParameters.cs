using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by StockTransferItemController to specify required parameters and sorting requirements
/// for the Stock Transfer reversal item list.
/// </summary>
public class StockTransferReversalItemsParameters
{
    /// <summary>
    /// Stock transfer reversal JobNo
    /// </summary>
    public string JobNo { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }
}



