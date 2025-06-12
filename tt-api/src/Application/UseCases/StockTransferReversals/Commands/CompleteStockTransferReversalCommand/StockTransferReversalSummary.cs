namespace Application.UseCases.StockTransferReversals.Commands.CompleteStockTransferReversalCommand;

public class StockTransferReversalSummary
{
    public string JobNo { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public int TotalQty { get; set; }
    public int TotalPkg { get; set; }
}
