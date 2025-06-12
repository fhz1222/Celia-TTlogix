namespace Application.UseCases.InboundReversalItems.Queries.GetReversibleInboundItems;

public class GetReversibleInboundItemsDtoFilter
{
    public string? PID { get; set; } = null!;
    public string? ProductCode { get; set; } = null!;
    public string? LocationCode { get; set; } = null!;
}
