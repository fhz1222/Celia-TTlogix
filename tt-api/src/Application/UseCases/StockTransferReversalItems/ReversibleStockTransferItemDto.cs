namespace Application.UseCases.StockTransferReversalItems;

public class ReversibleStockTransferItemDto
{
    public string PID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public int Qty { get; set; }
    public string LocationCode { get; set; } = null!;
    public string OriginalLocationCode { get; set; } = null!;
}
