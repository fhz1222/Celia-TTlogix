using Domain.Enums;

namespace Application.UseCases.StockTransferReversals;

public class StockTransferInfo
{
    public string? RefNo { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public StockTransferStatus Status { get; set; }
    public StockTransferType Type { get; set; }
}
