namespace Presentation.Common;

/// <summary>
/// This class is used by StockTransferItemController to specify required parameters
/// for the Stock Transfer reversal item add endpoint.
/// </summary>
public class StockTransferReversalItemsAddParameters
{
    /// <summary>
    /// Stock transfer reversal JobNo
    /// </summary>
    public string JobNo { get; set; } = null!;

    /// <summary>
    /// PIDs to create items for
    /// </summary>
    public string[] PIDs { get; set; } = null!;
}



