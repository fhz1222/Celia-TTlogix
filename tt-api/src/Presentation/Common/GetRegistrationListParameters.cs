using Application.Common.Models;

namespace Presentation.Common;

/// <summary>
/// This class is used by Registration Controller to specify required parameters, filters, sorting and pagination requirements
/// for the different lists.
/// </summary>
public class GetRegistrationListParameters
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
    public int? Type { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Specifies the field used for sorting and if it is ascending (default) or descending  
    /// </summary>  
    public OrderBy? Sorting { get; set; }
}



