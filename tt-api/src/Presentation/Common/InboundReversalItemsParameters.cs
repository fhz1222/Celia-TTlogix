using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by InboundReversalController to specify required parameters, filters, sorting and pagination requirements
/// for the inbound reversal items list.
/// </summary>
public class InboundReversalItemsParameters
{
    /// <summary>
    /// Inbound Reversal JobNo
    /// </summary>
    public string JobNo { get; set; } = null!;

    /// <summary>
    /// PID filter
    /// </summary>
    public string? PID { get; set; }

    /// <summary>
    /// Product Code filter
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



