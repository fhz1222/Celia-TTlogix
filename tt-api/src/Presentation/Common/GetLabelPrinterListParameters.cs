using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by Registration Controller to specify required parameters, filters, sorting and pagination requirements
/// for the different lists.
/// </summary>
public class GetLabelPrinterListParameters
{
    /// <summary>
    /// IP
    /// </summary>
    public string? Ip { get; set; } = null!;

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; } = null!;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; } = null!;

    /// <summary>
    /// Type  0 - CL412, 1 - CL4NX
    /// </summary>
    public int? Type { get; set; } = null!;

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }
}



