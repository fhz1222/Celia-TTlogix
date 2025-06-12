using Application.Common.Models;

namespace Application.UseCases.InboundReversals.Queries.GetReversibleInbounds;

public class GetReversibleInboundsDtoFilter
{
    public PaginationQuery Pagination { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string? InJobNo { get; set; } = null!;
    public DateTime? NewerThan { get; set; }
}
