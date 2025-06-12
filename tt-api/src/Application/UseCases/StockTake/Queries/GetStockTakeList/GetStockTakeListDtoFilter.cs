namespace Application.UseCases.StockTake.Queries.GetStockTakeList;

public class GetStockTakeListDtoFilter
{
    public string? JobNo { get; set; }
    public string? RefNo { get; set; }
    public string WhsCode { get; set; } = null!;
    public string? LocationCode { get; set; }
    public DtoFilterDateTimeRange? CreatedDate { get; set; }
    public IEnumerable<int>? Statuses { get; set; }
    public string? Remark { get; set; }
}
