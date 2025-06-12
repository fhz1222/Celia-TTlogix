namespace Application.UseCases.StockTransferReversalItems.Commands.AddNewStockTransferReversalItemCommand;

public class StockTransferDetailInfo
{
    public string OriginalSupplierID { get; set; } = null!;
    public string LocationCode { get; set; } = null!;
    public string OriginalLocationCode { get; set; } = null!;
    public string WHSCode { get; set; } = null!;
    public string OriginalWHSCode { get; set; } = null!;
    public string ProductCode { get; set; } = null!;
}
