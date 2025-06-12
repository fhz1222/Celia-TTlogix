using Domain.Enums;

namespace Application.UseCases.StockTransferReversals;

public class StockTransferReversalDetailsDto
{
    public string JobNo { get; set; } = null!;
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string WhsCode { get; set; } = null!;
    public string RefNo { get; set; } = null!;
    public string StfJobNo { get; set; } = null!;
    public string? Reason { get; set; }
    public StockTransferReversalStatus Status { get; set; }
    public StockTransferType Type { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedDate { get; set; }
}
