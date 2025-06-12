namespace Application.Common.Models;

/// <summary>
/// Specifies page number and size of page
/// </summary>
public class PaginationQuery
{
    /// <summary>
    /// Requested page number
    /// </summary>
    public int PageNumber { get; set; }
    /// <summary>
    /// Requested number of pages
    /// </summary>
    public int ItemsPerPage { get; set; }
}
