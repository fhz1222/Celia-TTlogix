namespace Application.UseCases.StockTransferReversalItems;

public class StockTransferReversalItemDto
{
    public string PID { get; set; } = null!;
    public string OriginalSupplierID { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Qty { get; set; }
    public string NewWhsCode { get; set; } = null!;
    public string NewLocationCode { get; set; } = null!;
}
