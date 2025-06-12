namespace Application.UseCases.StockTransferReversals.Queries.GetStockTransferReversals;

public class GetStockTransferReversalsDtoFilter
{
    public string? JobNo { get; set; }
    public string? WhsCode { get; set; }
    public string? CustomerCode { get; set; }
    public string? RefNo { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    public IEnumerable<int>? Statuses { get; set; }
    public string? Reason { get; set; }
}
