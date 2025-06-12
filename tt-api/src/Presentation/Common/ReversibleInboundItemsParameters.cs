using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by InboundReversalController to specify required parameters, filters, sorting and pagination requirements
/// for the reversible inbound items list.
/// </summary>
public class ReversibleInboundItemsParameters
{
    /// <summary>
    /// Inbound JobNo
    /// </summary>
    public string InJobNo { get; set; } = null!;

    /// <summary>
    /// PID filter
    /// </summary>
    public string? PID { get; set; }

    /// <summary>
    /// Product Code filter
    /// </summary>
    public string? ProductCode { get; set; }

    /// <summary>
    /// Location Code filter
    /// </summary>
    public string? LocationCode { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>
    public OrderBy? Sorting { get; set; }
}



