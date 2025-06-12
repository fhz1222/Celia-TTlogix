namespace Presentation.Common;

/// <summary>
/// This class is used by InboundReversalController to specify required parameters
/// for the inbound reversal items add endpoint.
/// </summary>
public class InboundReversalItemsAddParameters
{
    /// <summary>
    /// Inbound Reversal JobNo
    /// </summary>
    public string JobNo { get; set; } = null!;

    /// <summary>
    /// PIDs to add items for
    /// </summary>
    public string[] PIDs { get; set; } = null!;
}



