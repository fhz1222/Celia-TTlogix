using Domain.Enums;

namespace Application.UseCases.StockTransferReversals;

public class StockTransferReversalDto
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public StockTransferReversalStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Reason { get; set; }
}
