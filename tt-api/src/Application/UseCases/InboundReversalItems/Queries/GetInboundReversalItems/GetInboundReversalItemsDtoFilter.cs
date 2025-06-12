using Application.Common.Models;

namespace Application.UseCases.InboundReversalItems.Queries.GetInboundReversalItems;

public class GetInboundReversalItemsDtoFilter
{
    public string? PID { get; set; } = null!;
    public string? ProductCode { get; set; } = null!;

}
