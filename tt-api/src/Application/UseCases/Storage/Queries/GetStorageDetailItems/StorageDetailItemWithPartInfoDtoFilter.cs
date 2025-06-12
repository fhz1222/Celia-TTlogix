using Application.Common.Models;

namespace Application.UseCases.Storage.Queries.GetStorageDetailItems;

public class StorageDetailItemWithPartInfoDtoFilter
{
    public string WhsCode { get; set; } = null!;
    public string? OutboundJobNo { get; set; }
    public int LineItem { get; set; }

    public string? PID { get; set; }
    public DateTime? InboundDate { get; set; }
    public string? Location { get; set; }
    public string? GroupID { get; set; }
    public PaginationQuery Pagination { get; set; } = null!;

}