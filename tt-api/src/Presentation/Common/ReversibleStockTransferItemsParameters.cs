using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by StockTransferReversalController to specify required parameters, filters, sorting and pagination requirements
/// for the reversible Stock Transfer items list.
/// </summary>
public class ReversibleStockTransferItemsParameters
{
    /// <summary>
    /// Stock transfer JobNo
    /// </summary>
    public string StfJobNo { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



